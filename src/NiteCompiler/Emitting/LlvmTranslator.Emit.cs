using System;
using System.Collections.Generic;
using LLVMSharp.Interop;
using NiteCompiler.CodeAnalysis;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.IntermediateRepresentation;
using NiteCompiler.IntermediateRepresentation.ControlFlow;
using NiteCompiler.IntermediateRepresentation.Nir;

namespace NiteCompiler.Emitting;

internal partial class LlvmTranslator
{
	private enum ComparisonKind : byte
	{
		Equal,
		NotEqual,
		GreaterThan,
		GreaterThanOrEqual,
		LessThan,
		LessThanOrEqual
	}

	private void EmitFunctionBody(FunctionPlan plan)
	{
		if (plan.Nir == null || plan.Cfg == null || plan.Function.IsExtern)
		{
			return;
		}

		LLVMValueRef llvmFunction = plan.LlvmFunction;
		Dictionary<BasicBlock, LLVMBasicBlockRef> blockMap = new(plan.Cfg.Blocks.Length);
		Dictionary<IValue, LLVMValueRef> valueMap = new();
		Dictionary<ParameterSymbol, LLVMValueRef> parameterMap = new();
		List<(NirPhi Phi, LLVMValueRef LlvmPhi)> pendingPhis = [];

		foreach (BasicBlock block in plan.Cfg.Blocks)
		{
			blockMap[block] = llvmFunction.AppendBasicBlock(block.Name ?? "block");
		}

		foreach (ParameterSymbol param in plan.Function.Parameters)
		{
			parameterMap[param] = llvmFunction.GetParam((uint)param.Ordinal);
		}

		NirFunction nir = plan.Nir;

		// Collect symbols whose address is taken (need stack slots)
		HashSet<LocalVariableOrParameterSymbol> addressTakenSymbols = new();
		foreach (NirBlock nirBlock in nir.Blocks.Values)
		{
			foreach (Instruction instruction in nirBlock.Instructions)
			{
				if (instruction is AddressOfInstruction addrOf)
				{
					addressTakenSymbols.Add(addrOf.Symbol);
				}
			}
		}

		// Position at entry block to create allocas
		_builder.PositionAtEnd(blockMap[plan.Cfg.Entry]);

		// Build address table: create allocas for address-taken symbols
		Dictionary<LocalVariableOrParameterSymbol, LLVMValueRef> addressTable = new();
		foreach (var symbol in addressTakenSymbols)
		{
			LLVMValueRef alloca = _builder.BuildAlloca(GetLlvmType(symbol.Type), symbol.Name);
			addressTable[symbol] = alloca;
		}

		// Store parameters into allocas for address-taken params
		foreach (var (param, paramValue) in parameterMap)
		{
			if (addressTable.ContainsKey(param))
			{
				_builder.BuildStore(paramValue, addressTable[param]);
			}
		}

		// Map Param values to their LLVM function params
		foreach (var (param, nirParam) in nir.Parameters)
		{
			valueMap[nirParam] = parameterMap[param];
		}

		foreach (BasicBlock cfgBlock in plan.Cfg.Blocks)
		{
			NirBlock nirBlock = nir.Blocks[cfgBlock];
			_builder.PositionAtEnd(blockMap[cfgBlock]);

			foreach (NirPhi phi in nirBlock.Phis)
			{
				LLVMValueRef phiNode = _builder.BuildPhi(GetLlvmType(phi.Variable.Type), phi.Variable.Name);
				if (phi.Result != null)
				{
					valueMap[phi.Result] = phiNode;
				}
				pendingPhis.Add((phi, phiNode));
			}

			foreach (Instruction instruction in nirBlock.Instructions)
			{
				EmitInstruction(instruction, blockMap, valueMap, parameterMap, addressTable);
			}
		}

		foreach ((NirPhi phi, LLVMValueRef llvmPhi) in pendingPhis)
		{
			LLVMValueRef[] values = new LLVMValueRef[phi.Inputs.Count];
			LLVMBasicBlockRef[] blocks = new LLVMBasicBlockRef[phi.Inputs.Count];
			int i = 0;
			foreach (var (block, value) in phi.Inputs)
			{
				values[i] = ResolveValue(value, valueMap);
				blocks[i] = blockMap[block];
				i++;
			}
			llvmPhi.AddIncoming(values, blocks, (uint)phi.Inputs.Count);
		}
	}

	private void EmitInstruction(Instruction instruction,
		Dictionary<BasicBlock, LLVMBasicBlockRef> blockMap,
		Dictionary<IValue, LLVMValueRef> valueMap,
		Dictionary<ParameterSymbol, LLVMValueRef> parameterMap,
		Dictionary<LocalVariableOrParameterSymbol, LLVMValueRef> addressTable)
	{
		switch (instruction)
		{
			case UndefInstruction undef:
				valueMap[undef.Output] = EmitDefaultValue(undef.Output.Type);
				break;
			case StackAllocInstruction alloca:
				valueMap[alloca.Output] = EmitStackAlloc(alloca);
				break;
			case LoadImmInstruction imm:
				valueMap[imm.Output] = EmitConstant(imm.Constant, imm.Output.Type);
				break;
			case LoadParamInstruction param:
				valueMap[param.Output] = EmitParam(param.Parameter, parameterMap);
				break;
			case AddInstruction add:
				valueMap[add.Output] = EmitAdd(add.Left.Value, add.Right.Value, add.Output.Type, "add", valueMap);
				break;
			case SubInstruction sub:
				valueMap[sub.Output] = EmitSub(sub.Left.Value, sub.Right.Value, sub.Output.Type, "sub", valueMap);
				break;
			case MulInstruction mul:
				valueMap[mul.Output] = EmitMul(mul.Left.Value, mul.Right.Value, mul.Output.Type, "mul", valueMap);
				break;
			case DivInstruction div:
				valueMap[div.Output] = EmitDiv(div.Left.Value, div.Right.Value, div.Output.Type, "div", valueMap);
				break;
			case ModInstruction mod:
				valueMap[mod.Output] = EmitRem(mod.Left.Value, mod.Right.Value, mod.Output.Type, "rem", valueMap);
				break;
			case NegInstruction neg:
				valueMap[neg.Output] = EmitNeg(neg.Input.Value, neg.Output.Type, "neg", valueMap);
				break;
			case NotInstruction not:
				valueMap[not.Output] = EmitNot(not.Input.Value, not.Output.Type, "not", valueMap);
				break;
			case AndInstruction and:
				valueMap[and.Output] = _builder.BuildAnd(ResolveValue(and.Left.Value, valueMap), ResolveValue(and.Right.Value, valueMap), "and");
				break;
			case OrInstruction or:
				valueMap[or.Output] = _builder.BuildOr(ResolveValue(or.Left.Value, valueMap), ResolveValue(or.Right.Value, valueMap), "or");
				break;
			case XorInstruction xor:
				valueMap[xor.Output] = _builder.BuildXor(ResolveValue(xor.Left.Value, valueMap), ResolveValue(xor.Right.Value, valueMap), "xor");
				break;
			case SalInstruction sal:
				valueMap[sal.Output] = _builder.BuildShl(ResolveValue(sal.Left.Value, valueMap), ResolveValue(sal.Right.Value, valueMap), "sal");
				break;
			case SarInstruction sar:
				valueMap[sar.Output] = _builder.BuildAShr(ResolveValue(sar.Left.Value, valueMap), ResolveValue(sar.Right.Value, valueMap), "sar");
				break;
			case ShrInstruction shr:
				valueMap[shr.Output] = _builder.BuildLShr(ResolveValue(shr.Left.Value, valueMap), ResolveValue(shr.Right.Value, valueMap), "shr");
				break;
			case CmpEqInstruction eq:
				valueMap[eq.Output] = EmitComparison(eq.Left.Value, eq.Right.Value, eq.Left.Value.Type, ComparisonKind.Equal, valueMap);
				break;
			case CmpNeqInstruction neq:
				valueMap[neq.Output] = EmitComparison(neq.Left.Value, neq.Right.Value, neq.Left.Value.Type, ComparisonKind.NotEqual, valueMap);
				break;
			case CmpGtInstruction gt:
				valueMap[gt.Output] = EmitComparison(gt.Left.Value, gt.Right.Value, gt.Left.Value.Type, ComparisonKind.GreaterThan, valueMap);
				break;
			case CmpGeInstruction ge:
				valueMap[ge.Output] = EmitComparison(ge.Left.Value, ge.Right.Value, ge.Left.Value.Type, ComparisonKind.GreaterThanOrEqual, valueMap);
				break;
			case CmpLtInstruction lt:
				valueMap[lt.Output] = EmitComparison(lt.Left.Value, lt.Right.Value, lt.Left.Value.Type, ComparisonKind.LessThan, valueMap);
				break;
			case CmpLeInstruction le:
				valueMap[le.Output] = EmitComparison(le.Left.Value, le.Right.Value, le.Left.Value.Type, ComparisonKind.LessThanOrEqual, valueMap);
				break;
			case CallInstruction call:
			{
				DeclareFunction(call.Function);
				var arguments = new LLVMValueRef[call.Arguments.Length];
				for (int i = 0; i < arguments.Length; i++)
				{
					arguments[i] = ResolveValue(call.Arguments[i].Value, valueMap);
				}

				LLVMValueRef ret = _builder.BuildCall2(
					CreateFunctionType(call.Function),
					_plans[call.Function].LlvmFunction,
					arguments,
					call.Function.ReturnType.IsVoidType ? string.Empty : "call");

				if (call.Output != null)
				{
					valueMap[call.Output] = ret;
				}
				break;
			}
			case AddressOfInstruction addressOf:
				valueMap[addressOf.Output] = addressTable[addressOf.Symbol];
				break;
			case LoadInstruction load:
				valueMap[load.Output] = _builder.BuildLoad2(
					GetLlvmType(load.Output.Type),
					ResolveValue(load.Address.Value, valueMap),
					"load");
				break;
			case StoreInstruction store:
				_builder.BuildStore(ResolveValue(store.Value.Value, valueMap), ResolveValue(store.Address.Value, valueMap));
				break;
			case GetElementPointer gep:
			{
				LLVMValueRef basePtr = ResolveValue(gep.BaseAddress.Value, valueMap);
				TypeSymbol? containingType = gep.Field.ContainingType;
				if (containingType == null)
					throw new InvalidOperationException($"Field '{gep.Field.Name}' has no containing type.");
				LLVMTypeRef structType = GetLlvmType(containingType);
				int fieldIndex = FindFieldIndex(gep.Field);
				valueMap[gep.Output] = _builder.BuildStructGEP2(structType, basePtr, (uint)fieldIndex, "gep");
				break;
			}
			case RetInstruction ret:
				if (ret.Value == null)
				{
					_builder.BuildRetVoid();
				}
				else
				{
					_builder.BuildRet(ResolveValue(ret.Value.Value, valueMap));
				}
				break;
			case BrInstruction br:
				_builder.BuildBr(blockMap[br.Target]);
				break;
			case CondBrInstruction condBr:
				_builder.BuildCondBr(ResolveValue(condBr.Condition.Value, valueMap), blockMap[condBr.ThenBlock], blockMap[condBr.ElseBlock]);
				break;
			default:
				throw new NotSupportedException($"LLVM translation for '{instruction.GetType().Name}' is not implemented.");
		}
	}

	private LLVMValueRef EmitDefaultValue(TypeSymbol type)
	{
		LLVMTypeRef llvmType = GetLlvmType(type);
		SpecialType st = type.SpecialType;
		if (st.IsFloat)
		{
			return LLVMValueRef.CreateConstReal(llvmType, 0.0);
		}

		return LLVMValueRef.CreateConstInt(llvmType, 0, st.IsSignedIntegral);
	}

	private LLVMValueRef EmitStackAlloc(StackAllocInstruction stackAlloc)
	{
		return _builder.BuildAlloca(GetLlvmType(stackAlloc.TypeOf), "stackalloc");
	}

	private LLVMValueRef EmitParam(ParameterSymbol param, Dictionary<ParameterSymbol, LLVMValueRef> parameterMap)
	{
		return parameterMap[param];
	}

	private LLVMValueRef EmitConstant(ConstantValue constant, TypeSymbol type)
	{
		LLVMTypeRef llvmType = GetLlvmType(type);
		return constant.SpecialType switch
		{
			SpecialType.StdBoolean => LLVMValueRef.CreateConstInt(llvmType, constant.Bool ? 1ul : 0ul, false),
			SpecialType.StdNumericsSInt8 => LLVMValueRef.CreateConstInt(llvmType, unchecked((ulong)(long)constant.S8), true),
			SpecialType.StdNumericsSInt16 => LLVMValueRef.CreateConstInt(llvmType, unchecked((ulong)(long)constant.S16), true),
			SpecialType.StdNumericsSInt32 => LLVMValueRef.CreateConstInt(llvmType, unchecked((ulong)(long)constant.S32), true),
			SpecialType.StdNumericsSInt64 => LLVMValueRef.CreateConstInt(llvmType, unchecked((ulong)constant.S64), true),
			SpecialType.StdNumericsUInt8 => LLVMValueRef.CreateConstInt(llvmType, constant.U8, false),
			SpecialType.StdNumericsUInt16 => LLVMValueRef.CreateConstInt(llvmType, constant.U16, false),
			SpecialType.StdNumericsUInt32 => LLVMValueRef.CreateConstInt(llvmType, constant.U32, false),
			SpecialType.StdNumericsUInt64 => LLVMValueRef.CreateConstInt(llvmType, constant.U64, false),
			SpecialType.StdNumericsFloat32 => LLVMValueRef.CreateConstReal(llvmType, constant.F32),
			SpecialType.StdNumericsFloat64 => LLVMValueRef.CreateConstReal(llvmType, constant.F64),
			_ => throw new NotSupportedException($"Constant '{constant.SpecialType}' is not supported in LLVM translation.")
		};
	}

	private LLVMValueRef EmitAdd(IValue left, IValue right, TypeSymbol type, string name, Dictionary<IValue, LLVMValueRef> valueMap)
	{
		return type.SpecialType.IsFloat
			? _builder.BuildFAdd(ResolveValue(left, valueMap), ResolveValue(right, valueMap), name)
			: _builder.BuildAdd(ResolveValue(left, valueMap), ResolveValue(right, valueMap), name);
	}

	private LLVMValueRef EmitSub(IValue left, IValue right, TypeSymbol type, string name, Dictionary<IValue, LLVMValueRef> valueMap)
	{
		return type.SpecialType.IsFloat
			? _builder.BuildFSub(ResolveValue(left, valueMap), ResolveValue(right, valueMap), name)
			: _builder.BuildSub(ResolveValue(left, valueMap), ResolveValue(right, valueMap), name);
	}

	private LLVMValueRef EmitMul(IValue left, IValue right, TypeSymbol type, string name, Dictionary<IValue, LLVMValueRef> valueMap)
	{
		return type.SpecialType.IsFloat
			? _builder.BuildFMul(ResolveValue(left, valueMap), ResolveValue(right, valueMap), name)
			: _builder.BuildMul(ResolveValue(left, valueMap), ResolveValue(right, valueMap), name);
	}

	private LLVMValueRef EmitDiv(IValue left, IValue right, TypeSymbol type, string name, Dictionary<IValue, LLVMValueRef> valueMap)
	{
		if (type.SpecialType.IsFloat)
		{
			return _builder.BuildFDiv(ResolveValue(left, valueMap), ResolveValue(right, valueMap), name);
		}

		return type.SpecialType.IsUnsignedIntegral
			? _builder.BuildUDiv(ResolveValue(left, valueMap), ResolveValue(right, valueMap), name)
			: _builder.BuildSDiv(ResolveValue(left, valueMap), ResolveValue(right, valueMap), name);
	}

	private LLVMValueRef EmitRem(IValue left, IValue right, TypeSymbol type, string name, Dictionary<IValue, LLVMValueRef> valueMap)
	{
		if (type.SpecialType.IsFloat)
		{
			return _builder.BuildFRem(ResolveValue(left, valueMap), ResolveValue(right, valueMap), name);
		}

		return type.SpecialType.IsUnsignedIntegral
			? _builder.BuildURem(ResolveValue(left, valueMap), ResolveValue(right, valueMap), name)
			: _builder.BuildSRem(ResolveValue(left, valueMap), ResolveValue(right, valueMap), name);
	}

	private LLVMValueRef EmitNeg(IValue input, TypeSymbol type, string name, Dictionary<IValue, LLVMValueRef> valueMap)
	{
		return type.SpecialType.IsFloat
			? _builder.BuildFNeg(ResolveValue(input, valueMap), name)
			: _builder.BuildNeg(ResolveValue(input, valueMap), name);
	}

	private LLVMValueRef EmitNot(IValue input, TypeSymbol type, string name, Dictionary<IValue, LLVMValueRef> valueMap)
	{
		if (type.SpecialType == SpecialType.StdBoolean)
		{
			LLVMValueRef one = LLVMValueRef.CreateConstInt(GetLlvmType(type), 1, false);
			return _builder.BuildXor(ResolveValue(input, valueMap), one, name);
		}

		return _builder.BuildNot(ResolveValue(input, valueMap), name);
	}

	private LLVMValueRef EmitComparison(IValue left, IValue right, TypeSymbol operandType,
		ComparisonKind comparisonKind, Dictionary<IValue, LLVMValueRef> valueMap)
	{
		return operandType.SpecialType.IsFloat
			? _builder.BuildFCmp(GetRealPredicate(comparisonKind), ResolveValue(left, valueMap), ResolveValue(right, valueMap), "fcmp")
			: _builder.BuildICmp(GetIntPredicate(operandType.SpecialType, comparisonKind), ResolveValue(left, valueMap), ResolveValue(right, valueMap), "icmp");
	}

	private static LLVMValueRef ResolveValue(IValue value, Dictionary<IValue, LLVMValueRef> valueMap)
	{
		return valueMap[value];
	}

	private static LLVMIntPredicate GetIntPredicate(SpecialType operandType, ComparisonKind comparisonKind)
	{
		bool unsigned = operandType.IsUnsignedIntegral || operandType == SpecialType.StdBoolean;
		return comparisonKind switch
		{
			ComparisonKind.Equal => LLVMIntPredicate.LLVMIntEQ,
			ComparisonKind.NotEqual => LLVMIntPredicate.LLVMIntNE,
			ComparisonKind.GreaterThan => unsigned ? LLVMIntPredicate.LLVMIntUGT : LLVMIntPredicate.LLVMIntSGT,
			ComparisonKind.GreaterThanOrEqual => unsigned ? LLVMIntPredicate.LLVMIntUGE : LLVMIntPredicate.LLVMIntSGE,
			ComparisonKind.LessThan => unsigned ? LLVMIntPredicate.LLVMIntULT : LLVMIntPredicate.LLVMIntSLT,
			ComparisonKind.LessThanOrEqual => unsigned ? LLVMIntPredicate.LLVMIntULE : LLVMIntPredicate.LLVMIntSLE,
			_ => throw new ArgumentOutOfRangeException(nameof(comparisonKind))
		};
	}

	private static LLVMRealPredicate GetRealPredicate(ComparisonKind comparisonKind)
	{
		return comparisonKind switch
		{
			ComparisonKind.Equal => LLVMRealPredicate.LLVMRealOEQ,
			ComparisonKind.NotEqual => LLVMRealPredicate.LLVMRealONE,
			ComparisonKind.GreaterThan => LLVMRealPredicate.LLVMRealOGT,
			ComparisonKind.GreaterThanOrEqual => LLVMRealPredicate.LLVMRealOGE,
			ComparisonKind.LessThan => LLVMRealPredicate.LLVMRealOLT,
			ComparisonKind.LessThanOrEqual => LLVMRealPredicate.LLVMRealOLE,
			_ => throw new ArgumentOutOfRangeException(nameof(comparisonKind))
		};
	}

	private LLVMTypeRef CreateFunctionType(FunctionSymbol function)
	{
		var parameterTypes = new LLVMTypeRef[function.Parameters.Length];
		for (int i = 0; i < parameterTypes.Length; i++)
		{
			parameterTypes[i] = GetLlvmType(function.Parameters[i].Type);
		}

		return LLVMTypeRef.CreateFunction(GetLlvmType(function.ReturnType), parameterTypes, IsVarArg: false);
	}

	private LLVMTypeRef GetLlvmType(TypeSymbol type)
	{
		if (type is BaseReferenceTypeSymbol referenceType)
		{
			return LLVMTypeRef.CreatePointer(GetLlvmType(referenceType.PointsTo), 0);
		}

		if (type.SpecialType != SpecialType.None)
		{
			return type.SpecialType switch
			{
				SpecialType.StdNumericsSInt8 or SpecialType.StdNumericsUInt8 => _context.Int8Type,
				SpecialType.StdNumericsSInt16 or SpecialType.StdNumericsUInt16 => _context.Int16Type,
				SpecialType.StdNumericsSInt32 or SpecialType.StdNumericsUInt32 => _context.Int32Type,
				SpecialType.StdNumericsSInt64 or SpecialType.StdNumericsUInt64 => _context.Int64Type,
				// TODO: receive bitness from target
				SpecialType.StdNumericsSNativeInt or SpecialType.StdNumericsUNativeInt => _context.Int64Type,
				SpecialType.StdNumericsFloat16 => _context.HalfType,
				SpecialType.StdNumericsFloat32 => _context.FloatType,
				SpecialType.StdNumericsFloat64 => _context.DoubleType,
				SpecialType.StdBoolean => _context.Int1Type,
				SpecialType.StdVoid or SpecialType.StdNeverReturn => _context.VoidType,
				_ => throw new NotSupportedException($"Type '{type.ToDisplayString()}' is not supported in LLVM translation.")
			};
		}

		if (type is NamedTypeSymbol namedType)
		{
			return GetOrCreateStructType(namedType);
		}

		throw new NotSupportedException($"Type '{type.ToDisplayString()}' is not supported in LLVM translation.");
	}

	private LLVMTypeRef GetOrCreateStructType(NamedTypeSymbol namedType)
	{
		if (_structTypes.TryGetValue(namedType, out LLVMTypeRef cached))
			return cached;

		var fieldTypes = new List<LLVMTypeRef>();
		foreach (Symbol member in namedType.GetMembers())
		{
			if (member is FieldSymbol field)
			{
				fieldTypes.Add(GetLlvmType(field.Type));
			}
		}

		LLVMTypeRef namedStruct = _context.CreateNamedStruct(namedType.ToDisplayString(SymbolFormat.Metadata));
		namedStruct.StructSetBody([..fieldTypes], false);
		_structTypes[namedType] = namedStruct;
		return namedStruct;
	}

	private static int FindFieldIndex(FieldSymbol field)
	{
		TypeSymbol? containingType = field.ContainingType;
		if (containingType is not NamedTypeSymbol namedType)
			throw new InvalidOperationException($"Field '{field.Name}' does not belong to a named type.");
		int index = 0;
		foreach (Symbol member in namedType.GetMembers())
		{
			if (member == field)
				return index;
			if (member is FieldSymbol)
				index++;
		}

		throw new InvalidOperationException($"Field '{field.Name}' not found in type '{namedType.Name}'.");
	}
}
