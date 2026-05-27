using System;
using System.Collections.Generic;
using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Binding.Conversions;
using NiteCompiler.CodeAnalysis.Binding.Operators;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.IntermediateRepresentation.ControlFlow;

namespace NiteCompiler.IntermediateRepresentation.Nir;

internal sealed class NirBuilder
{
	private readonly HashSet<LocalVariableOrParameterSymbol> _memoryMapped = [];
	private readonly Dictionary<LocalVariableOrParameterSymbol, Temp> _addressMap = [];
	private readonly Dictionary<LocalVariableOrParameterSymbol, Stack<IValue>> _stacks = new();
	private int _tempId = 0;

	public static NirFunction Build(ControlFlowGraph cfg, FunctionSymbol function)
	{
		Debug.Assert(function != null);

		NirBuilder builder = new();
		return builder.CreateNir(cfg, function);
	}

	private NirFunction CreateNir(ControlFlowGraph cfg, FunctionSymbol function)
	{
		NirFunction nir = new(function);

		foreach (var block in cfg.Blocks)
		{
			nir.Blocks[block] = new NirBlock(block);
		}

		FindMemoryMappedSymbols(cfg, function);

		foreach (var v in CollectAllVariablesAndParameters(cfg, function))
		{
			_stacks[v] = new Stack<IValue>();
		}

		NirBlock entryBlock = nir.Blocks[cfg.Entry];
		foreach (var v in _memoryMapped)
		{
			Temp addr = NewTemp(v.Type);
			entryBlock.Instructions.Add(new StackAllocInstruction(addr, v.Type));
			_addressMap[v] = addr;
		}

		// For memory-mapped parameters, store the initial param value to the stack slot
		foreach (ParameterSymbol param in function.Parameters)
		{
			if (_memoryMapped.Contains(param))
			{
				Param paramValue = new(param);
				entryBlock.Instructions.Add(new StoreInstruction(new Copy(paramValue), new Copy(_addressMap[param])));
			}
		}

		var defs = CollectDefinitions(cfg);

		InsertPhiNodes(cfg, defs, nir);

		InitializeParameters(function, nir);

		Rename(cfg.Entry, cfg.DominatorTree, nir);

		return nir;
	}

	private Temp NewTemp(TypeSymbol typeOf) => new(_tempId++, typeOf);

	private IEnumerable<LocalVariableOrParameterSymbol> CollectAllVariablesAndParameters(ControlFlowGraph cfg, FunctionSymbol function)
	{
		HashSet<LocalVariableOrParameterSymbol> set = [];

		foreach (ParameterSymbol p in function.Parameters)
		{
			set.Add(p);
		}

		foreach (var block in cfg.Blocks)
		{
			foreach (var stmt in block.Statements)
			{
				if (stmt is BoundLocalVariableDeclarationStatement var)
				{
					set.Add(var.Local);
				}
			}
		}

		return set;
	}

	private void FindMemoryMappedSymbols(ControlFlowGraph cfg, FunctionSymbol function)
	{
		foreach (var block in cfg.Blocks)
		{
			foreach (BoundStatement stmt in block.Statements)
			{
				if (stmt is BoundExpressionStatement { Expression: BoundAddressOfExpression addrOf })
				{
					var symbol = addrOf.Expression switch
					{
						BoundMove move => move.Variable,
						BoundCopy copy => copy.Variable,
						BoundParameter param => param.Variable,
						BoundLocalVariable localVar => localVar.Variable,
						_ => null
					};
					if (symbol != null)
						_memoryMapped.Add(symbol);
				}

				if (stmt is BoundLocalVariableDeclarationStatement { Initializer: BoundCall { Function.IsConstructor: true } } decl)
				{
					_memoryMapped.Add(decl.Local);
				}

				if (stmt is BoundExpressionStatement { Expression: BoundAssignment { Left: var left, Right: BoundCall { Function.IsConstructor: true } } })
				{
					var symbol = left switch
					{
						BoundMove move => move.Variable,
						BoundCopy copy => copy.Variable,
						BoundParameter param => param.Variable,
						BoundLocalVariable localVar => localVar.Variable,
						_ => null
					};
					if (symbol != null)
						_memoryMapped.Add(symbol);
				}
			}
		}
	}

	private Dictionary<LocalVariableOrParameterSymbol, HashSet<BasicBlock>> CollectDefinitions(ControlFlowGraph cfg)
	{
		var result = new Dictionary<LocalVariableOrParameterSymbol, HashSet<BasicBlock>>();

		foreach (var block in cfg.Blocks)
		{
			foreach (BoundStatement stmt in block.Statements)
			{
				if (stmt is BoundLocalVariableDeclarationStatement varDeclaration)
				{
					LocalVariableSymbol var = varDeclaration.Local;
					if (_memoryMapped.Contains(var)) continue;
					if (!result.TryGetValue(var, out HashSet<BasicBlock>? set))
					{
						set = [];
						result[var] = set;
					}
					set.Add(block);
				}

				if (stmt is BoundExpressionStatement { Expression: BoundAssignment assign })
				{
					BoundExpression target = assign.Left;

				LocalVariableOrParameterSymbol? targetSymbol = target switch
				{
					BoundMove move => move.Variable,
					BoundCopy copy => copy.Variable,
					BoundLocalVariable lv => lv.Variable,
					BoundParameter p => p.Variable,
					_ => null
				};

					if (targetSymbol == null) continue;
					if (_memoryMapped.Contains(targetSymbol)) continue;

					if (!result.TryGetValue(targetSymbol, out HashSet<BasicBlock>? set))
					{
						set = [];
						result[targetSymbol] = set;
					}
					set.Add(block);
				}
			}
		}

		return result;
	}

	private void InitializeParameters(FunctionSymbol function, NirFunction nir)
	{
		foreach (ParameterSymbol parameter in function.Parameters)
		{
			Param value = new(parameter);
			nir.Parameters[parameter] = value;

			if (!_memoryMapped.Contains(parameter))
			{
				_stacks[parameter].Push(value);
			}
		}
	}

	private void InsertPhiNodes(ControlFlowGraph cfg,
		Dictionary<LocalVariableOrParameterSymbol, HashSet<BasicBlock>> defs, NirFunction nir)
	{
		var df = cfg.DominanceFrontier;

		foreach (var (variable, defBlocks) in defs)
		{
			if (_memoryMapped.Contains(variable)) continue;
			Queue<BasicBlock> worklist = new(defBlocks);
			HashSet<BasicBlock> hasAlready = [];

			while (worklist.Count > 0)
			{
				BasicBlock block = worklist.Dequeue();

				if (!df.TryGetValue(block, out var frontier))
					continue;

				foreach (var y in frontier)
				{
					if (hasAlready.Contains(y))
						continue;

					NirBlock nirBlock = nir.Blocks[y];

					NirPhi phi = new(variable);
					nirBlock.Phis.Add(phi);

					hasAlready.Add(y);

					if (!defBlocks.Contains(y))
					{
						worklist.Enqueue(y);
					}
				}
			}
		}
	}

	private void Rename(
		BasicBlock block,
		IReadOnlyDictionary<BasicBlock, List<BasicBlock>> domTree,
		NirFunction function)
	{
		var snapshot = SaveStacks();

		var nirBlock = function.Blocks[block];

		foreach (var phi in nirBlock.Phis)
		{
			if (_memoryMapped.Contains(phi.Variable)) continue;
			Temp temp = NewTemp(phi.Variable.Type);
			phi.Result = temp;
			_stacks[phi.Variable].Push(temp);
		}

		foreach (BoundStatement statement in block.Statements)
		{
			RewriteStatement(statement, nirBlock);
		}
		Debug.Assert(block.Terminator is not null);
		{
			RewriteTerminator(block.Terminator, nirBlock);
		}

		foreach (BasicBlock successor in block.Successors)
		{
			NirBlock successorNir = function.Blocks[successor];

			foreach (var phi in successorNir.Phis)
			{
				if (_memoryMapped.Contains(phi.Variable)) continue;
				IValue value = _stacks[phi.Variable].Peek();
				phi.Inputs.Add(block, value);
			}
		}

		foreach (BasicBlock child in domTree[block])
		{
			Rename(child, domTree, function);
		}

		RestoreStacks(snapshot);
	}

	private void RewriteTerminator(ControlFlowTerminator terminator, NirBlock block)
	{
		switch (terminator)
		{
			case ConditionalBranchTerminator condBr:
				Operand cond = RewriteExpression(condBr.Condition, block);
				block.Instructions.Add(new CondBrInstruction(cond, condBr.Then, condBr.Else));
				break;
			case ReturnTerminator ret:
				if (ret.Expression != null)
				{
					Operand retValue = RewriteExpression(ret.Expression, block);
					block.Instructions.Add(new RetInstruction(retValue));
				}
				else
				{
					block.Instructions.Add(new RetInstruction(null));
				}
				break;
			case BranchTerminator br:
				block.Instructions.Add(new BrInstruction(br.Target));
				break;
			default:
				throw new UnreachableException($"RewriteTerminator({terminator.GetType()})");
		}
	}

	private void RewriteStatement(BoundStatement statement, NirBlock block)
	{
		switch (statement)
		{
			case BoundExpressionStatement es:
				RewriteExpression(es.Expression, block);
				break;
			case BoundLocalVariableDeclarationStatement var:
				if (var.Initializer != null)
				{
					Operand operand = RewriteExpression(var.Initializer, block);
					if (_memoryMapped.Contains(var.Local))
					{
						block.Instructions.Add(new StoreInstruction(operand, new Copy(_addressMap[var.Local])));
					}
					else
					{
						_stacks[var.Local].Push(operand.Value);
					}
				}
				break;
			case BoundEmptyStatement:
				break;
			default:
				throw new UnreachableException($"RewriteStatement({statement.GetType()})");
		}
	}

	private Operand RewriteExpression(BoundExpression expression, NirBlock block)
	{
		return expression switch
		{
			BoundLiteral literal => EmitLiteral(literal, block),
			BoundUnaryExpression unary => EmitUnaryExpression(unary, block),
			BoundBinaryExpression binary => EmitBinaryExpression(binary, block),
			BoundAssignment assignment => EmitAssignmentExpression(assignment, block),
			BoundCompoundAssignment compoundAssignment => throw new InvalidOperationException("Unlowered BoundTree is passed to the NirBuilder."),
			BoundMove move => EmitLocal(move, block),
			BoundCopy copy => EmitLocal(copy, block),
			BoundParameter param => EmitLocal(param, block),
			BoundLocalVariable localVar => EmitLocal(localVar, block),
			BoundCall call => EmitCall(call, block),
			BoundAddressOfExpression addressOf => EmitAddressOfExpression(addressOf, block),
			BoundDereferenceExpression dereference => EmitDereferenceExpression(dereference, block),
			BoundFieldAccess fieldAccess => EmitFieldAccess(fieldAccess, block),
			BoundConversion conversion => EmitConversion(conversion, block),
		BoundCollectionExpression coll => EmitCollectionExpression(coll, block),

			_ => throw new UnreachableException($"RewriteExpression({expression.GetType()})")
		};
	}

	private Operand EmitLiteral(BoundLiteral literal, NirBlock block)
	{
		if (literal.Type is BaseReferenceTypeSymbol { IsThickPointer: true } refType)
		{
			string text = literal.ConstantValue.StringValue;
			var encoding = refType.PointsTo.SpecialType switch
			{
				SpecialType.StdTextStringSliceUtf16 => StringLiteralType.Unicode16,
				SpecialType.StdTextStringSliceUtf32 => StringLiteralType.Unicode32,
				_ => StringLiteralType.Unicode8
			};
			Temp output = NewTemp(literal.Type);
			LoadStringInstruction str = new(output, text, encoding);
			block.Instructions.Add(str);
			return new Copy(output);
		}

		switch (literal.Type.SpecialType)
		{
			case (>= SpecialType.StdNumericsSInt8 and <= SpecialType.StdNumericsFloat64)
				or SpecialType.StdBoolean:
			{
				Temp output = NewTemp(literal.Type);
				LoadImmInstruction imm = new(output, literal.ConstantValue);
				block.Instructions.Add(imm);
				return new Copy(output);
			}
			case not SpecialType.None:
				throw new ArgumentException($"Imposible literal type: {literal.Type.SpecialType}");
			default:
				throw new UnreachableException($"EmitLiteral({literal.Type.ToDisplayString()})");
		}
	}

	private Operand EmitUnaryExpression(BoundUnaryExpression unary, NirBlock block)
	{
		if (unary.Op.CorrespondingFunction is not null)
		{
			throw new NotImplementedException("call instruction is not implemented yet");
		}

		Operand value = RewriteExpression(unary.Expression, block);

		if (unary.Op.Kind == UnaryOperatorKind.Plus)
		{
			return value;
		}

		Temp result = NewTemp(unary.Type);
		UnaryInstruction instruction = unary.Op.Kind switch
		{
			UnaryOperatorKind.Negate => new NegInstruction(result, value),
			UnaryOperatorKind.BitwiseNot or UnaryOperatorKind.LogicalNot => new NotInstruction(result, value),
			_ => throw new UnreachableException($"EmitUnaryExpression({unary.Op.Kind})")
		};
		block.Instructions.Add(instruction);
		return new Copy(result);
	}

	private Operand EmitBinaryExpression(BoundBinaryExpression binary, NirBlock block)
	{
		if (binary.Op.CorrespondingFunction is not null)
		{
			throw new NotImplementedException("call instruction is not implemented yet");
		}

		Operand lhs = RewriteExpression(binary.Left, block);
		Operand rhs = RewriteExpression(binary.Right, block);
		Temp result = NewTemp(binary.Type);

		BinaryInstruction instruction = binary.Op.Kind switch
		{
			BinaryOperatorKind.Addition => new AddInstruction(result, lhs, rhs),
			BinaryOperatorKind.Subtraction => new SubInstruction(result, lhs, rhs),
			BinaryOperatorKind.Multiplication => new MulInstruction(result, lhs, rhs),
			BinaryOperatorKind.Division => new DivInstruction(result, lhs, rhs),
			BinaryOperatorKind.Modulo => new ModInstruction(result, lhs, rhs),
			BinaryOperatorKind.LeftArithmeticShift => new SalInstruction(result, lhs, rhs),
			BinaryOperatorKind.RightArithmeticShift => new SarInstruction(result, lhs, rhs),
			BinaryOperatorKind.RightUnsignedShift => new ShrInstruction(result, lhs, rhs),
			BinaryOperatorKind.Equal => new CmpEqInstruction(result, lhs, rhs),
			BinaryOperatorKind.NotEqual => new CmpNeqInstruction(result, lhs, rhs),
			BinaryOperatorKind.Greater => new CmpGtInstruction(result, lhs, rhs),
			BinaryOperatorKind.Less => new CmpLtInstruction(result, lhs, rhs),
			BinaryOperatorKind.GreaterOrEqual => new CmpGeInstruction(result, lhs, rhs),
			BinaryOperatorKind.LessOrEqual => new CmpLeInstruction(result, lhs, rhs),
			BinaryOperatorKind.And => new AndInstruction(result, lhs, rhs),
			BinaryOperatorKind.Xor => new XorInstruction(result, lhs, rhs),
			BinaryOperatorKind.Or => new OrInstruction(result, lhs, rhs),
			_ => throw new UnreachableException($"EmitBinaryExpression({binary.Op.Kind})")
		};

		block.Instructions.Add(instruction);
		return new Copy(result);
	}

	private Operand EmitAssignmentExpression(BoundAssignment assignment, NirBlock block)
	{
		Operand right = RewriteExpression(assignment.Right, block);

		LocalVariableOrParameterSymbol? symbol = assignment.Left switch
		{
			BoundMove move => move.Variable,
			BoundCopy copy => copy.Variable,
			BoundParameter param => param.Variable,
			BoundLocalVariable localVar => localVar.Variable,
			BoundDereferenceExpression => null,
			BoundFieldAccess => null,
			_ => throw new UnreachableException($"EmitAssignmentExpression({assignment.Left.GetType()})")
		};

		if (symbol != null)
		{
			if (_memoryMapped.Contains(symbol))
			{
				block.Instructions.Add(new StoreInstruction(right, new Copy(_addressMap[symbol])));
			}
			else
			{
				_stacks[symbol].Push(right.Value);
			}
			return right;
		}

		if (assignment.Left is BoundDereferenceExpression storeDeref)
		{
			Operand addr = RewriteExpression(storeDeref.Expression, block);
			block.Instructions.Add(new StoreInstruction(right, addr));
		}
		else if (assignment.Left is BoundFieldAccess fieldAccess)
		{
			if (fieldAccess.Receiver.Type is BaseReferenceTypeSymbol)
			{
				Operand basePtr = GetReceiverPointer(fieldAccess.Receiver, block);
				Temp fieldPtr = NewTemp(fieldAccess.Type);
				block.Instructions.Add(new GetElementPointer(fieldPtr, basePtr, fieldAccess.Field));
				block.Instructions.Add(new StoreInstruction(right, new Copy(fieldPtr)));
			}
			else
			{
				Operand aggregate = GetReceiverValue(fieldAccess.Receiver, block);
				Temp result = NewTemp(aggregate.Value.Type);
				block.Instructions.Add(new InsertValueInstruction(result, aggregate, right, fieldAccess.Field));

				LocalVariableOrParameterSymbol? fieldSymbol = fieldAccess.Receiver switch
				{
					BoundMove move => move.Variable,
					BoundCopy copy => copy.Variable,
					BoundParameter param => param.Variable,
					BoundLocalVariable localVar => localVar.Variable,
					_ => null
				};

				if (fieldSymbol != null)
				{
					if (_memoryMapped.Contains(fieldSymbol))
					{
						block.Instructions.Add(new StoreInstruction(new Copy(result), new Copy(_addressMap[fieldSymbol])));
					}
					else
					{
						_stacks[fieldSymbol].Push(result);
					}
				}
				else if (fieldAccess.Receiver is BoundDereferenceExpression)
				{
					Operand ptr = GetReceiverPointer(fieldAccess.Receiver, block);
					block.Instructions.Add(new StoreInstruction(new Copy(result), ptr));
				}
			}
		}

		return right;
	}

	private Operand EmitLocal(BoundMove move, NirBlock block)
	{
		if (_memoryMapped.Contains(move.Variable))
		{
			Temp loaded = NewTemp(move.Variable.Type);
			block.Instructions.Add(new LoadInstruction(loaded, new Copy(_addressMap[move.Variable])));
			return new Move(loaded);
		}
		return new Move(ReadLocal(move.Variable));
	}

	private Operand EmitLocal(BoundCopy copy, NirBlock block)
	{
		if (_memoryMapped.Contains(copy.Variable))
		{
			Temp loaded = NewTemp(copy.Variable.Type);
			block.Instructions.Add(new LoadInstruction(loaded, new Copy(_addressMap[copy.Variable])));
			return new Copy(loaded);
		}
		return new Copy(ReadLocal(copy.Variable));
	}

	private Operand EmitLocal(BoundParameter param, NirBlock block)
	{
		if (_memoryMapped.Contains(param.Variable))
		{
			Temp loaded = NewTemp(param.Variable.Type);
			block.Instructions.Add(new LoadInstruction(loaded, new Copy(_addressMap[param.Variable])));
			return new Copy(loaded);
		}
		return new Copy(ReadLocal(param.Variable));
	}

	private Operand EmitLocal(BoundLocalVariable localVar, NirBlock block)
	{
		if (_memoryMapped.Contains(localVar.Variable))
		{
			Temp loaded = NewTemp(localVar.Variable.Type);
			block.Instructions.Add(new LoadInstruction(loaded, new Copy(_addressMap[localVar.Variable])));
			return new Copy(loaded);
		}
		return new Copy(ReadLocal(localVar.Variable));
	}

	private Operand EmitConversion(BoundConversion conversion, NirBlock block)
	{
		if (conversion.ConversionKind == ConversionKind.NoConversion || conversion.ConversionKind == ConversionKind.Identity)
		{
			return RewriteExpression(conversion.Operand, block);
		}

		Operand operand = RewriteExpression(conversion.Operand, block);
		Temp result = NewTemp(conversion.Type);
		UnaryInstruction instruction;

		if (conversion.ConversionKind == ConversionKind.ImplicitNumeric)
		{
			instruction = conversion.Operand.Type.SpecialType.IsSignedIntegral
				? new SExtInstruction(result, operand)
				: new ZExtInstruction(result, operand);
		}
		else if (conversion.ConversionKind == ConversionKind.ImplicitFloatExtension)
		{
			instruction = new FPExtInstruction(result, operand);
		}
		else if (conversion.ConversionKind == ConversionKind.ImplicitSignedIntegerToFloat)
		{
			instruction = new SIToFPInstruction(result, operand);
		}
		else if (conversion.ConversionKind == ConversionKind.ExplicitNumericTruncate)
		{
			instruction = new TruncInstruction(result, operand);
		}
		else if (conversion.ConversionKind is ConversionKind.ExplicitNumericSExt)
		{
			instruction = new SExtInstruction(result, operand);
		}
		else if (conversion.ConversionKind is ConversionKind.ExplicitNumericZExt)
		{
			instruction = new ZExtInstruction(result, operand);
		}
		else if (conversion.ConversionKind is ConversionKind.ExplicitSignedToUnsigned or ConversionKind.ExplicitUnsignedToSigned)
		{
			// Same-size bit reinterpret — handled implicitly by NIR/LLVM type system
			return operand;
		}
		else if (conversion.ConversionKind == ConversionKind.ExplicitUnsignedIntegerToFloat)
		{
			instruction = new UIToFPInstruction(result, operand);
		}
		else if (conversion.ConversionKind == ConversionKind.ExplicitFloatToInteger)
		{
			instruction = conversion.Type.SpecialType.IsSignedIntegral
				? new FPToSIInstruction(result, operand)
				: new FPToUIInstruction(result, operand);
		}
		else if (conversion.ConversionKind == ConversionKind.ExplicitFloatTruncate)
		{
			instruction = new FPTruncInstruction(result, operand);
		}
		else
		{
			throw new UnreachableException($"EmitConversion({conversion.ConversionKind})");
		}

		block.Instructions.Add(instruction);
		return new Copy(result);
	}

	private Operand EmitCall(BoundCall call, NirBlock block)
	{
		if (call.Function.IsConstructor)
		{
			var ctor = (ConstructorSymbol)call.Function;
			Temp resultAddr = NewTemp(ctor.SelfParameter.Type);
			block.Instructions.Add(new StackAllocInstruction(resultAddr, call.Type));

			var ctorArgs = new Operand[call.Arguments.Length + 1];
			ctorArgs[0] = new Copy(resultAddr);
			for (int i = 0; i < call.Arguments.Length; i++)
			{
				ctorArgs[i + 1] = RewriteExpression(call.Arguments[i], block);
			}

			block.Instructions.Add(new CallInstruction(null, call.Function, [..ctorArgs]));

			Temp loaded = NewTemp(call.Type);
			block.Instructions.Add(new LoadInstruction(loaded, new Copy(resultAddr)));
			return new Copy(loaded);
		}

		Operand[] arguments = new Operand[call.Arguments.Length];
		for (int i = 0; i < arguments.Length; i++)
		{
			arguments[i] = RewriteExpression(call.Arguments[i], block);
		}
		Temp ret = NewTemp(call.Type);

		block.Instructions.Add(new CallInstruction(ret, call.Function, [..arguments]));
		return new Copy(ret);
	}

	private Operand EmitAddressOfExpression(BoundAddressOfExpression addressOf, NirBlock block)
	{
		LocalVariableOrParameterSymbol symbol = addressOf.Expression switch
		{
			BoundMove move => move.Variable,
			BoundCopy copy => copy.Variable,
			BoundParameter param => param.Variable,
			BoundLocalVariable localVar => localVar.Variable,
			_ => throw new UnreachableException($"EmitAddressOfExpression({addressOf.Expression.GetType()})")
		};
		if (_memoryMapped.Contains(symbol))
		{
			return new Copy(_addressMap[symbol]);
		}
		Temp output = NewTemp(addressOf.Type);
		block.Instructions.Add(new AddressOfInstruction(output, symbol));
		return new Copy(output);
	}

	private Operand EmitDereferenceExpression(BoundDereferenceExpression dereference, NirBlock block)
	{
		Operand addr = RewriteExpression(dereference.Expression, block);
		Temp load = NewTemp(dereference.Type);
		block.Instructions.Add(new LoadInstruction(load, addr));
		return new Copy(load);
	}

	private Operand EmitFieldAccess(BoundFieldAccess fieldAccess, NirBlock block)
	{
		if (fieldAccess.Receiver.Type is BaseReferenceTypeSymbol)
		{
			Operand basePtr = GetReceiverPointer(fieldAccess.Receiver, block);
			Temp fieldPtr = NewTemp(fieldAccess.Type);
			block.Instructions.Add(new GetElementPointer(fieldPtr, basePtr, fieldAccess.Field));
			Temp result = NewTemp(fieldAccess.Type);
			block.Instructions.Add(new LoadInstruction(result, new Copy(fieldPtr)));
			return new Copy(result);
		}
		Operand aggregate = GetReceiverValue(fieldAccess.Receiver, block);
		Temp result2 = NewTemp(fieldAccess.Type);
		block.Instructions.Add(new ExtractValueInstruction(result2, aggregate, fieldAccess.Field));
		return new Copy(result2);
	}

	private Operand EmitCollectionExpression(BoundCollectionExpression coll, NirBlock block)
	{
		throw new NotImplementedException("Collection expression codegen is not yet implemented.");
	}

	private Operand GetReceiverPointer(BoundExpression receiver, NirBlock block)
	{
		switch (receiver)
		{
			case BoundMove move when move.Variable.Type is BaseReferenceTypeSymbol:
				return EmitLocal(move, block);
			case BoundCopy copy when copy.Variable.Type is BaseReferenceTypeSymbol:
				return EmitLocal(copy, block);
			case BoundParameter param when param.Variable.Type is BaseReferenceTypeSymbol:
				return EmitLocal(param, block);
			case BoundLocalVariable localVar when localVar.Variable.Type is BaseReferenceTypeSymbol:
				return EmitLocal(localVar, block);
		case BoundMove move:
			if (_memoryMapped.Contains(move.Variable))
				return new Copy(_addressMap[move.Variable]);
		{
			Temp addr = NewTemp(move.Variable.Type);
			block.Instructions.Add(new AddressOfInstruction(addr, move.Variable));
			IValue current = ReadLocal(move.Variable);
			block.Instructions.Add(new StoreInstruction(new Copy(current), new Copy(addr)));
			return new Copy(addr);
		}
		case BoundCopy copy:
			if (_memoryMapped.Contains(copy.Variable))
				return new Copy(_addressMap[copy.Variable]);
		{
			Temp addr = NewTemp(copy.Variable.Type);
			block.Instructions.Add(new AddressOfInstruction(addr, copy.Variable));
			IValue current = ReadLocal(copy.Variable);
			block.Instructions.Add(new StoreInstruction(new Copy(current), new Copy(addr)));
			return new Copy(addr);
		}
		case BoundParameter param:
			if (_memoryMapped.Contains(param.Variable))
				return new Copy(_addressMap[param.Variable]);
		{
			Temp addr = NewTemp(param.Variable.Type);
			block.Instructions.Add(new AddressOfInstruction(addr, param.Variable));
			IValue current = ReadLocal(param.Variable);
			block.Instructions.Add(new StoreInstruction(new Copy(current), new Copy(addr)));
			return new Copy(addr);
		}
		case BoundLocalVariable localVar:
			if (_memoryMapped.Contains(localVar.Variable))
				return new Copy(_addressMap[localVar.Variable]);
		{
			Temp addr = NewTemp(localVar.Variable.Type);
			block.Instructions.Add(new AddressOfInstruction(addr, localVar.Variable));
			IValue current = ReadLocal(localVar.Variable);
			block.Instructions.Add(new StoreInstruction(new Copy(current), new Copy(addr)));
			return new Copy(addr);
		}
			case BoundDereferenceExpression deref:
				return RewriteExpression(deref.Expression, block);
			default:
				throw new NotImplementedException($"GetReceiverPointer: {receiver.GetType()}");
		}
	}

	private Operand GetReceiverValue(BoundExpression receiver, NirBlock block)
	{
		switch (receiver)
		{
			case BoundMove move when move.Variable.Type is BaseReferenceTypeSymbol:
			{
				Operand ptr = EmitLocal(move, block);
				TypeSymbol pointsTo = ((BaseReferenceTypeSymbol)move.Variable.Type).PointsTo;
				Temp loaded = NewTemp(pointsTo);
				block.Instructions.Add(new LoadInstruction(loaded, ptr));
				return new Copy(loaded);
			}
			case BoundCopy copy when copy.Variable.Type is BaseReferenceTypeSymbol:
			{
				Operand ptr = EmitLocal(copy, block);
				TypeSymbol pointsTo = ((BaseReferenceTypeSymbol)copy.Variable.Type).PointsTo;
				Temp loaded = NewTemp(pointsTo);
				block.Instructions.Add(new LoadInstruction(loaded, ptr));
				return new Copy(loaded);
			}
			case BoundParameter param when param.Variable.Type is BaseReferenceTypeSymbol:
			{
				Operand ptr = EmitLocal(param, block);
				TypeSymbol pointsTo = ((BaseReferenceTypeSymbol)param.Variable.Type).PointsTo;
				Temp loaded = NewTemp(pointsTo);
				block.Instructions.Add(new LoadInstruction(loaded, ptr));
				return new Copy(loaded);
			}
			case BoundLocalVariable localVar when localVar.Variable.Type is BaseReferenceTypeSymbol:
			{
				Operand ptr = EmitLocal(localVar, block);
				TypeSymbol pointsTo = ((BaseReferenceTypeSymbol)localVar.Variable.Type).PointsTo;
				Temp loaded = NewTemp(pointsTo);
				block.Instructions.Add(new LoadInstruction(loaded, ptr));
				return new Copy(loaded);
			}
			case BoundMove move:
				return EmitLocal(move, block);
			case BoundCopy copy:
				return EmitLocal(copy, block);
			case BoundParameter param:
				return EmitLocal(param, block);
			case BoundLocalVariable localVar:
				return EmitLocal(localVar, block);
			case BoundDereferenceExpression deref:
				return RewriteExpression(deref, block);
			default:
				throw new NotImplementedException($"GetReceiverValue: {receiver.GetType()}");
		}
	}

	private Dictionary<LocalVariableOrParameterSymbol, int> SaveStacks()
	{
		Dictionary<LocalVariableOrParameterSymbol, int> snapshot = new();

		foreach (var (k, v) in _stacks)
		{
			if (_memoryMapped.Contains(k)) continue;
			snapshot[k] = v.Count;
		}

		return snapshot;
	}

	private void RestoreStacks(Dictionary<LocalVariableOrParameterSymbol, int> snapshot)
	{
		foreach (var (k, count) in snapshot)
		{
			var stack = _stacks[k];
			while (stack.Count > count)
				stack.Pop();
		}
	}

	private IValue ReadLocal(LocalVariableOrParameterSymbol symbol)
	{
		if (!_stacks.TryGetValue(symbol, out var stack) || stack.Count == 0)
		{
			throw new InvalidOperationException($"'{symbol.Name}' is not initialized in current state => invalid bound tree or critical errors during SSA-translation. This is compiler error!");
		}

		return stack.Peek();
	}
}
