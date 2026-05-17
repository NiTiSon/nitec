using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Binding.Operators;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.IntermediateRepresentation.ControlFlow;

namespace NiteCompiler.IntermediateRepresentation.Nir;

internal sealed class NirBuilder
{
	private readonly Dictionary<LocalVariableOrParameterSymbol, Stack<IValue>> _stacks = new();
	private int _tempId = 0;

	public static NirFunction Build(ControlFlowGraph cfg, FunctionSymbol function)
	{
		var builder = new NirBuilder();
		return builder.CreateNir(cfg, function);
	}

	private NirFunction CreateNir(ControlFlowGraph cfg, FunctionSymbol function)
	{
		NirFunction nir = new(function);

		foreach (var block in cfg.Blocks)
		{
			nir.Blocks[block] = new NirBlock(block);
		}

		foreach (var v in CollectAllVariablesAndParameters(cfg, function))
		{
			_stacks[v] = new Stack<IValue>();
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

	private static Dictionary<LocalVariableOrParameterSymbol, HashSet<BasicBlock>> CollectDefinitions(ControlFlowGraph cfg)
	{
		var result = new Dictionary<LocalVariableOrParameterSymbol, HashSet<BasicBlock>>();

		foreach (var block in cfg.Blocks)
		{
			foreach (BoundStatement stmt in block.Statements)
			{
				if (stmt is BoundLocalVariableDeclarationStatement varDeclaration)
				{
					LocalVariableSymbol var = varDeclaration.Local;
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
						_ => null
					};

					if (targetSymbol == null) continue;

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
			_stacks[parameter].Push(value);
			nir.Parameters[parameter] = value;
		}
	}

	private void InsertPhiNodes(ControlFlowGraph cfg,
		Dictionary<LocalVariableOrParameterSymbol, HashSet<BasicBlock>> defs, NirFunction nir)
	{
		var df = cfg.DominanceFrontier;

		foreach (var (variable, defBlocks) in defs)
		{
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
					_stacks[var.Local].Push(operand.Value);
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
			BoundCall call => EmitCall(call, block),
			BoundAddressOfExpression addressOf => EmitAddressOfExpression(addressOf, block),
			BoundDereferenceExpression dereference => EmitDereferenceExpression(dereference, block),

			_ => throw new UnreachableException($"RewriteExpression({expression.GetType()})")
		};
	}

	private Operand EmitLiteral(BoundLiteral literal, NirBlock block)
	{
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
			BoundDereferenceExpression => null,
			_ => throw new UnreachableException($"EmitAssignmentExpression({assignment.Left.GetType()})")
		};

		if (symbol != null)
		{
			_stacks[symbol].Push(right.Value);
			return right;
		}

		if (assignment.Left is BoundDereferenceExpression storeDeref)
		{
			Operand addr = RewriteExpression(storeDeref.Expression, block);
			block.Instructions.Add(new StoreInstruction(right, addr));
		}

		return right;
	}

	private Operand EmitLocal(BoundMove move, NirBlock block)
	{
		return new Move(ReadLocal(move.Variable));
	}

	private Operand EmitLocal(BoundCopy copy, NirBlock block)
	{
		return new Copy(ReadLocal(copy.Variable));
	}

	private Operand EmitCall(BoundCall call, NirBlock block)
	{
		Temp ret = NewTemp(call.Type);
		Operand[] arguments = new Operand[call.Arguments.Length];
		for (int i = 0; i < arguments.Length; i++)
		{
			arguments[i] = RewriteExpression(call.Arguments[i], block);
		}

		block.Instructions.Add(new CallInstruction(ret, call.Function, [.. arguments]));
		return new Copy(ret);
	}

	private Operand EmitAddressOfExpression(BoundAddressOfExpression addressOf, NirBlock block)
	{
		Temp output = NewTemp(addressOf.Type);
		LocalVariableOrParameterSymbol symbol = addressOf.Expression switch
		{
			BoundMove move => move.Variable,
			BoundCopy copy => copy.Variable,
			_ => throw new UnreachableException($"EmitAddressOfExpression({addressOf.Expression.GetType()})")
		};
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

	private Dictionary<LocalVariableOrParameterSymbol, int> SaveStacks()
	{
		Dictionary<LocalVariableOrParameterSymbol, int> snapshot = new();

		foreach (var (k, v) in _stacks)
		{
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
