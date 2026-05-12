using System;
using System.Collections.Generic;
using LLVMSharp.Interop;
using NiteCompiler.CodeAnalysis;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.IntermediateRepresentation;
using NiteCompiler.IntermediateRepresentation.ControlFlow;
using NiteCompiler.IntermediateRepresentation.Ssa;

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
		if (plan.Ssa == null || plan.Cfg == null || plan.Function.IsExtern)
		{
			return;
		}

		LLVMValueRef llvmFunction = plan.LlvmFunction;
		Dictionary<BasicBlock, LLVMBasicBlockRef> blockMap = new(plan.Cfg.Blocks.Length);
		Dictionary<SsaValue, LLVMValueRef> valueMap = new();
		Dictionary<LocalVariableOrParameterSymbol, LLVMValueRef> storageMap = new();

		foreach (BasicBlock block in plan.Cfg.Blocks)
		{
			blockMap[block] = llvmFunction.AppendBasicBlock(block.Name ?? "block");
		}

		foreach (ParameterSymbol parameter in plan.Function.Parameters)
		{
			SsaParameter ssaParameter = new(parameter);
			valueMap[ssaParameter] = llvmFunction.GetParam((uint)parameter.Ordinal);
		}

		_builder.PositionAtEnd(blockMap[plan.Cfg.Entry]);
		foreach (LocalVariableOrParameterSymbol symbol in CollectAddressBackedSymbols(plan.Ssa))
		{
			LLVMValueRef storage = _builder.BuildAlloca(GetLlvmType(symbol.Type), $"{symbol.Name}.addr");
			storageMap[symbol] = storage;
			if (symbol is ParameterSymbol parameter)
			{
				_builder.BuildStore(llvmFunction.GetParam((uint)parameter.Ordinal), storage);
			}
		}

		foreach ((BasicBlock cfgBlock, SsaBlock ssaBlock) in plan.Ssa.Blocks)
		{
			_builder.PositionAtEnd(blockMap[cfgBlock]);

			foreach (SsaPhi phi in ssaBlock.Phis)
			{
				string phiName = phi.Result is null ? phi.Variable.Name : $"phi.{phi.Result.Id}";
				valueMap[phi.Result!] = _builder.BuildPhi(GetLlvmType(phi.Type), phiName);
			}

			foreach (Instruction instruction in ssaBlock.Instructions)
			{
				EmitInstruction(instruction, blockMap, valueMap, storageMap);
			}
		}

		foreach (SsaBlock ssaBlock in plan.Ssa.Blocks.Values)
		{
			foreach (SsaPhi phi in ssaBlock.Phis)
			{
				var incomingValues = new LLVMValueRef[phi.Inputs.Count];
				var incomingBlocks = new LLVMBasicBlockRef[phi.Inputs.Count];
				int index = 0;

				foreach ((BasicBlock predecessor, SsaValue value) in phi.Inputs)
				{
					incomingValues[index] = ResolveValue(value, valueMap);
					incomingBlocks[index] = blockMap[predecessor];
					index++;
				}

				valueMap[phi.Result!].AddIncoming(incomingValues, incomingBlocks, (uint)incomingValues.Length);
			}
		}
	}

	private void EmitInstruction(Instruction instruction,
		Dictionary<BasicBlock, LLVMBasicBlockRef> blockMap,
		Dictionary<SsaValue, LLVMValueRef> valueMap,
		Dictionary<LocalVariableOrParameterSymbol, LLVMValueRef> storageMap)
	{
		switch (instruction)
		{
			case LoadImmInstruction imm:
				valueMap[imm.Output] = EmitConstant(imm.Constant, imm.Output.Type);
				break;
			case AddInstruction add:
				valueMap[add.Output] = EmitAdd(add.Left, add.Right, add.Output.Type, "add", valueMap);
				break;
			case SubInstruction sub:
				valueMap[sub.Output] = EmitSub(sub.Left, sub.Right, sub.Output.Type, "sub", valueMap);
				break;
			case MulInstruction mul:
				valueMap[mul.Output] = EmitMul(mul.Left, mul.Right, mul.Output.Type, "mul", valueMap);
				break;
			case DivInstruction div:
				valueMap[div.Output] = EmitDiv(div.Left, div.Right, div.Output.Type, "div", valueMap);
				break;
			case ModInstruction mod:
				valueMap[mod.Output] = EmitRem(mod.Left, mod.Right, mod.Output.Type, "rem", valueMap);
				break;
			case NegInstruction neg:
				valueMap[neg.Output] = EmitNeg(neg.Input, neg.Output.Type, "neg", valueMap);
				break;
			case NotInstruction not:
				valueMap[not.Output] = EmitNot(not.Input, not.Output.Type, "not", valueMap);
				break;
			case AndInstruction and:
				valueMap[and.Output] = _builder.BuildAnd(ResolveValue(and.Left, valueMap), ResolveValue(and.Right, valueMap), "and");
				break;
			case OrInstruction or:
				valueMap[or.Output] = _builder.BuildOr(ResolveValue(or.Left, valueMap), ResolveValue(or.Right, valueMap), "or");
				break;
			case XorInstruction xor:
				valueMap[xor.Output] = _builder.BuildXor(ResolveValue(xor.Left, valueMap), ResolveValue(xor.Right, valueMap), "xor");
				break;
			case SalInstruction sal:
				valueMap[sal.Output] = _builder.BuildShl(ResolveValue(sal.Left, valueMap), ResolveValue(sal.Right, valueMap), "sal");
				break;
			case SarInstruction sar:
				valueMap[sar.Output] = _builder.BuildAShr(ResolveValue(sar.Left, valueMap), ResolveValue(sar.Right, valueMap), "sar");
				break;
			case ShrInstruction shr:
				valueMap[shr.Output] = _builder.BuildLShr(ResolveValue(shr.Left, valueMap), ResolveValue(shr.Right, valueMap), "shr");
				break;
			case CmpEqInstruction eq:
				valueMap[eq.Output] = EmitComparison(eq.Left, eq.Right, eq.Left.Type, ComparisonKind.Equal, valueMap);
				break;
			case CmpNeqInstruction neq:
				valueMap[neq.Output] = EmitComparison(neq.Left, neq.Right, neq.Left.Type, ComparisonKind.NotEqual, valueMap);
				break;
			case CmpGtInstruction gt:
				valueMap[gt.Output] = EmitComparison(gt.Left, gt.Right, gt.Left.Type, ComparisonKind.GreaterThan, valueMap);
				break;
			case CmpGeInstruction ge:
				valueMap[ge.Output] = EmitComparison(ge.Left, ge.Right, ge.Left.Type, ComparisonKind.GreaterThanOrEqual, valueMap);
				break;
			case CmpLtInstruction lt:
				valueMap[lt.Output] = EmitComparison(lt.Left, lt.Right, lt.Left.Type, ComparisonKind.LessThan, valueMap);
				break;
			case CmpLeInstruction le:
				valueMap[le.Output] = EmitComparison(le.Left, le.Right, le.Left.Type, ComparisonKind.LessThanOrEqual, valueMap);
				break;
			case CallInstruction call:
				DeclareFunction(call.Function);
				var arguments = new LLVMValueRef[call.Arguments.Length];
				for (int i = 0; i < arguments.Length; i++)
				{
					arguments[i] = ResolveValue(call.Arguments[i], valueMap);
				}

				valueMap[call.Output] = _builder.BuildCall2(
					CreateFunctionType(call.Function),
					_plans[call.Function].LlvmFunction,
					arguments,
					call.Output.Type.IsVoidType ? string.Empty : "call");
				break;
			case AddressOfInstruction addressOf:
				valueMap[addressOf.Output] = storageMap[addressOf.Symbol];
				break;
			case LoadLocalAddressInstruction loadLocal:
				valueMap[loadLocal.Output] = _builder.BuildLoad2(
					GetLlvmType(loadLocal.Output.Type),
					storageMap[loadLocal.Symbol],
					$"load.{loadLocal.Symbol.Name}");
				break;
			case StoreLocalAddressInstruction storeLocal:
				_builder.BuildStore(ResolveValue(storeLocal.Value, valueMap), storageMap[storeLocal.Symbol]);
				break;
			case LoadIndirectInstruction loadIndirect:
				valueMap[loadIndirect.Output] = _builder.BuildLoad2(
					GetLlvmType(loadIndirect.Output.Type),
					ResolveValue(loadIndirect.Address, valueMap),
					"load.ind");
				break;
			case StoreIndirectInstruction storeIndirect:
				_builder.BuildStore(ResolveValue(storeIndirect.Value, valueMap), ResolveValue(storeIndirect.Address, valueMap));
				break;
			case RetInstruction ret:
				if (ret.Value == null)
				{
					_builder.BuildRetVoid();
				}
				else
				{
					_builder.BuildRet(ResolveValue(ret.Value, valueMap));
				}
				break;
			case BrInstruction br:
				_builder.BuildBr(blockMap[br.Target]);
				break;
			case CondBrInstruction condBr:
				_builder.BuildCondBr(ResolveValue(condBr.Condition, valueMap), blockMap[condBr.ThenBlock], blockMap[condBr.ElseBlock]);
				break;
			default:
				throw new NotSupportedException($"LLVM translation for '{instruction.GetType().Name}' is not implemented.");
		}
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

	private LLVMValueRef EmitAdd(SsaValue left, SsaValue right, TypeSymbol type, string name, Dictionary<SsaValue, LLVMValueRef> valueMap)
	{
		return type.SpecialType.IsFloat
			? _builder.BuildFAdd(ResolveValue(left, valueMap), ResolveValue(right, valueMap), name)
			: _builder.BuildAdd(ResolveValue(left, valueMap), ResolveValue(right, valueMap), name);
	}

	private LLVMValueRef EmitSub(SsaValue left, SsaValue right, TypeSymbol type, string name, Dictionary<SsaValue, LLVMValueRef> valueMap)
	{
		return type.SpecialType.IsFloat
			? _builder.BuildFSub(ResolveValue(left, valueMap), ResolveValue(right, valueMap), name)
			: _builder.BuildSub(ResolveValue(left, valueMap), ResolveValue(right, valueMap), name);
	}

	private LLVMValueRef EmitMul(SsaValue left, SsaValue right, TypeSymbol type, string name, Dictionary<SsaValue, LLVMValueRef> valueMap)
	{
		return type.SpecialType.IsFloat
			? _builder.BuildFMul(ResolveValue(left, valueMap), ResolveValue(right, valueMap), name)
			: _builder.BuildMul(ResolveValue(left, valueMap), ResolveValue(right, valueMap), name);
	}

	private LLVMValueRef EmitDiv(SsaValue left, SsaValue right, TypeSymbol type, string name, Dictionary<SsaValue, LLVMValueRef> valueMap)
	{
		if (type.SpecialType.IsFloat)
		{
			return _builder.BuildFDiv(ResolveValue(left, valueMap), ResolveValue(right, valueMap), name);
		}

		return type.SpecialType.IsUnsignedIntegral
			? _builder.BuildUDiv(ResolveValue(left, valueMap), ResolveValue(right, valueMap), name)
			: _builder.BuildSDiv(ResolveValue(left, valueMap), ResolveValue(right, valueMap), name);
	}

	private LLVMValueRef EmitRem(SsaValue left, SsaValue right, TypeSymbol type, string name, Dictionary<SsaValue, LLVMValueRef> valueMap)
	{
		if (type.SpecialType.IsFloat)
		{
			return _builder.BuildFRem(ResolveValue(left, valueMap), ResolveValue(right, valueMap), name);
		}

		return type.SpecialType.IsUnsignedIntegral
			? _builder.BuildURem(ResolveValue(left, valueMap), ResolveValue(right, valueMap), name)
			: _builder.BuildSRem(ResolveValue(left, valueMap), ResolveValue(right, valueMap), name);
	}

	private LLVMValueRef EmitNeg(SsaValue input, TypeSymbol type, string name, Dictionary<SsaValue, LLVMValueRef> valueMap)
	{
		return type.SpecialType.IsFloat
			? _builder.BuildFNeg(ResolveValue(input, valueMap), name)
			: _builder.BuildNeg(ResolveValue(input, valueMap), name);
	}

	private LLVMValueRef EmitNot(SsaValue input, TypeSymbol type, string name, Dictionary<SsaValue, LLVMValueRef> valueMap)
	{
		if (type.SpecialType == SpecialType.StdBoolean)
		{
			LLVMValueRef one = LLVMValueRef.CreateConstInt(GetLlvmType(type), 1, false);
			return _builder.BuildXor(ResolveValue(input, valueMap), one, name);
		}

		return _builder.BuildNot(ResolveValue(input, valueMap), name);
	}

	private LLVMValueRef EmitComparison(SsaValue left, SsaValue right, TypeSymbol operandType,
		ComparisonKind comparisonKind, Dictionary<SsaValue, LLVMValueRef> valueMap)
	{
		return operandType.SpecialType.IsFloat
			? _builder.BuildFCmp(GetRealPredicate(comparisonKind), ResolveValue(left, valueMap), ResolveValue(right, valueMap), "fcmp")
			: _builder.BuildICmp(GetIntPredicate(operandType.SpecialType, comparisonKind), ResolveValue(left, valueMap), ResolveValue(right, valueMap), "icmp");
	}

	private LLVMValueRef ResolveValue(SsaValue value, Dictionary<SsaValue, LLVMValueRef> valueMap)
	{
		if (valueMap.TryGetValue(value, out LLVMValueRef llvmValue))
		{
			return llvmValue;
		}

		if (value is SsaUndef undef)
		{
			return LLVMValueRef.CreateConstNull(GetLlvmType(undef.Type));
		}

		if (value is SsaParameter parameter)
		{
			foreach ((SsaValue knownValue, LLVMValueRef knownLlvmValue) in valueMap)
			{
				if (knownValue is SsaParameter knownParameter && ReferenceEquals(knownParameter.Symbol, parameter.Symbol))
				{
					return knownLlvmValue;
				}
			}
		}

		throw new KeyNotFoundException($"No LLVM value mapped for SSA value '{value.GetType().Name}'.");
	}

	private static IEnumerable<LocalVariableOrParameterSymbol> CollectAddressBackedSymbols(SsaFunction ssa)
	{
		HashSet<LocalVariableOrParameterSymbol> result = [];
		foreach (SsaBlock block in ssa.Blocks.Values)
		{
			foreach (Instruction instruction in block.Instructions)
			{
				switch (instruction)
				{
					case AddressOfInstruction addressOf:
						result.Add(addressOf.Symbol);
						break;
					case LoadLocalAddressInstruction loadLocal:
						result.Add(loadLocal.Symbol);
						break;
					case StoreLocalAddressInstruction storeLocal:
						result.Add(storeLocal.Symbol);
						break;
				}
			}
		}

		return result;
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
}
