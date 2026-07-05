using System;
using System.Collections.Generic;
using LLVMSharp.Interop;
using NiteCompiler.CodeAnalysis;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.IntermediateRepresentation;
using NiteCompiler.IntermediateRepresentation.ControlFlow;
using NiteCompiler.IntermediateRepresentation.Mir;

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
		if (plan.Mir == null || plan.Cfg == null || plan.Function.IsExtern)
		{
			return;
		}

		LLVMValueRef llvmFunction = plan.LlvmFunction;
		Dictionary<BasicBlock, LLVMBasicBlockRef> blockMap = new(plan.Cfg.Blocks.Length);
		Dictionary<TempValue, LLVMValueRef> valueMap = new();
		Dictionary<ParameterSymbol, LLVMValueRef> parameterMap = new();

		foreach (BasicBlock block in plan.Cfg.Blocks)
		{
			blockMap[block] = llvmFunction.AppendBasicBlock(block.Name ?? "block");
		}

		foreach (ParameterSymbol param in plan.Function.Parameters)
		{
			parameterMap[param] = llvmFunction.GetParam((uint)param.Ordinal);
		}

		FunctionMir mir = plan.Mir;
		foreach (BasicBlock cfgBlock in plan.Cfg.Blocks)
		{
			MirBlock mirBlock = mir.Blocks[cfgBlock];
			_builder.PositionAtEnd(blockMap[cfgBlock]);

			foreach (Instruction instruction in mirBlock.Instructions)
			{
				EmitInstruction(instruction, blockMap, valueMap, parameterMap, mir);
			}
		}
	}

	private void EmitInstruction(Instruction instruction,
		Dictionary<BasicBlock, LLVMBasicBlockRef> blockMap,
		Dictionary<TempValue, LLVMValueRef> valueMap,
		Dictionary<ParameterSymbol, LLVMValueRef> parameterMap,
		FunctionMir mir)
	{
		switch (instruction)
		{
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
					call.Function.ReturnType == null ? string.Empty : "call");
				break;
			case AddressOfInstruction addressOf:
				valueMap[addressOf.Output] = ResolveValue(mir.AddressTable![addressOf.Symbol], valueMap);
				break;
			case LoadInstruction load:
				valueMap[load.Output] = _builder.BuildLoad2(
					GetLlvmType(load.Output.Type),
					ResolveValue(load.Address, valueMap),
					"load");
				break;
			case StoreInstruction store:
				_builder.BuildStore(ResolveValue(store.Value, valueMap), ResolveValue(store.Address, valueMap));
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

	private LLVMValueRef EmitAdd(TempValue left, TempValue right, TypeSymbol type, string name, Dictionary<TempValue, LLVMValueRef> valueMap)
	{
		return type.SpecialType.IsFloat
			? _builder.BuildFAdd(ResolveValue(left, valueMap), ResolveValue(right, valueMap), name)
			: _builder.BuildAdd(ResolveValue(left, valueMap), ResolveValue(right, valueMap), name);
	}

	private LLVMValueRef EmitSub(TempValue left, TempValue right, TypeSymbol type, string name, Dictionary<TempValue, LLVMValueRef> valueMap)
	{
		return type.SpecialType.IsFloat
			? _builder.BuildFSub(ResolveValue(left, valueMap), ResolveValue(right, valueMap), name)
			: _builder.BuildSub(ResolveValue(left, valueMap), ResolveValue(right, valueMap), name);
	}

	private LLVMValueRef EmitMul(TempValue left, TempValue right, TypeSymbol type, string name, Dictionary<TempValue, LLVMValueRef> valueMap)
	{
		return type.SpecialType.IsFloat
			? _builder.BuildFMul(ResolveValue(left, valueMap), ResolveValue(right, valueMap), name)
			: _builder.BuildMul(ResolveValue(left, valueMap), ResolveValue(right, valueMap), name);
	}

	private LLVMValueRef EmitDiv(TempValue left, TempValue right, TypeSymbol type, string name, Dictionary<TempValue, LLVMValueRef> valueMap)
	{
		if (type.SpecialType.IsFloat)
		{
			return _builder.BuildFDiv(ResolveValue(left, valueMap), ResolveValue(right, valueMap), name);
		}

		return type.SpecialType.IsUnsignedIntegral
			? _builder.BuildUDiv(ResolveValue(left, valueMap), ResolveValue(right, valueMap), name)
			: _builder.BuildSDiv(ResolveValue(left, valueMap), ResolveValue(right, valueMap), name);
	}

	private LLVMValueRef EmitRem(TempValue left, TempValue right, TypeSymbol type, string name, Dictionary<TempValue, LLVMValueRef> valueMap)
	{
		if (type.SpecialType.IsFloat)
		{
			return _builder.BuildFRem(ResolveValue(left, valueMap), ResolveValue(right, valueMap), name);
		}

		return type.SpecialType.IsUnsignedIntegral
			? _builder.BuildURem(ResolveValue(left, valueMap), ResolveValue(right, valueMap), name)
			: _builder.BuildSRem(ResolveValue(left, valueMap), ResolveValue(right, valueMap), name);
	}

	private LLVMValueRef EmitNeg(TempValue input, TypeSymbol type, string name, Dictionary<TempValue, LLVMValueRef> valueMap)
	{
		return type.SpecialType.IsFloat
			? _builder.BuildFNeg(ResolveValue(input, valueMap), name)
			: _builder.BuildNeg(ResolveValue(input, valueMap), name);
	}

	private LLVMValueRef EmitNot(TempValue input, TypeSymbol type, string name, Dictionary<TempValue, LLVMValueRef> valueMap)
	{
		if (type.SpecialType == SpecialType.StdBoolean)
		{
			LLVMValueRef one = LLVMValueRef.CreateConstInt(GetLlvmType(type), 1, false);
			return _builder.BuildXor(ResolveValue(input, valueMap), one, name);
		}

		return _builder.BuildNot(ResolveValue(input, valueMap), name);
	}

	private LLVMValueRef EmitComparison(TempValue left, TempValue right, TypeSymbol operandType,
		ComparisonKind comparisonKind, Dictionary<TempValue, LLVMValueRef> valueMap)
	{
		return operandType.SpecialType.IsFloat
			? _builder.BuildFCmp(GetRealPredicate(comparisonKind), ResolveValue(left, valueMap), ResolveValue(right, valueMap), "fcmp")
			: _builder.BuildICmp(GetIntPredicate(operandType.SpecialType, comparisonKind), ResolveValue(left, valueMap), ResolveValue(right, valueMap), "icmp");
	}

	private static LLVMValueRef ResolveValue(TempValue value, Dictionary<TempValue, LLVMValueRef> valueMap)
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

		LLVMTypeRef returnType = function.ReturnType != null ? GetLlvmType(function.ReturnType) : _context.VoidType;
		return LLVMTypeRef.CreateFunction(returnType, parameterTypes, IsVarArg: false);
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
			SpecialType.StdNeverReturn => _context.VoidType,
			_ => throw new NotSupportedException($"Type '{type.ToDisplayString()}' is not supported in LLVM translation.")
		};
	}
}
