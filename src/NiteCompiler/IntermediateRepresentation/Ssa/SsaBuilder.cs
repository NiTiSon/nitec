using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Binding.Operators;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.IntermediateRepresentation.ControlFlow;

namespace NiteCompiler.IntermediateRepresentation.Ssa;

internal sealed class SsaBuilder
{
	private readonly Dictionary<LocalVariableOrParameterSymbol, Stack<SsaValue>> _stacks = new();
	private int _tempId = 0;

	public static SsaFunction Build(ControlFlowGraph cfg, FunctionSymbol function)
	{
		var builder = new SsaBuilder();
		return builder.CreateSsa(cfg, function);
	}

	public SsaFunction CreateSsa(ControlFlowGraph cfg, FunctionSymbol function)
	{
		SsaFunction ssa = new(function);

		foreach (var block in cfg.Blocks)
		{
			ssa.Blocks[block] = new SsaBlock(block);
		}

		foreach (var v in CollectAllVariables(cfg, function)) // also collects parameters
		{
			_stacks[v] = new Stack<SsaValue>();
		}

		var defs = CollectDefinitions(cfg);

		InsertPhiNodes(cfg, defs);

		InitializeParameters(function);

		Rename(cfg.Entry, cfg.DominatorTree, ssa);

		return ssa;
	}

	private SsaTemp NewTemp(TypeSymbol typeOf) => new(typeOf, _tempId++);

	private IEnumerable<LocalVariableOrParameterSymbol> CollectAllVariables(ControlFlowGraph cfg, FunctionSymbol function)
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
				// TODO: Collect locals declarations

				// if (stmt is BoundExpressionStatement es &&
				//     es.Expression is BoundAssignment assign)
				// {
				// 	set.Add(assign.Target);
				// }
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
				if (stmt is BoundExpressionStatement { Expression: BoundAssignment assign })
				{
					BoundExpression target = assign.Left;

					LocalVariableOrParameterSymbol? targetSymbol = target switch
					{
						BoundLocal local => local.Local,
						BoundParameter param => param.Parameter,
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

	private void InitializeParameters(FunctionSymbol function)
	{
		foreach (ParameterSymbol parameter in function.Parameters)
		{
			SsaParameter value = new(parameter);
			_stacks[parameter].Push(value);
		}
	}

	private void InsertPhiNodes(ControlFlowGraph cfg, Dictionary<LocalVariableOrParameterSymbol, HashSet<BasicBlock>> defs)
	{
	}

	private void Rename(
		BasicBlock block,
		IReadOnlyDictionary<BasicBlock, List<BasicBlock>> domTree,
		SsaFunction function)
	{
		var snapshot = SaveStacks();

		var ssaBlock = function.Blocks[block];

		foreach (var phi in ssaBlock.Phis)
		{
			var temp = NewTemp(phi.Type);
			phi.Result = temp;
			_stacks[phi.Variable].Push(temp);
		}

		foreach (BoundStatement statement in block.Statements)
		{
			RewriteStatement(statement, ssaBlock);
		}
		Debug.Assert(block.Terminator is not null);
		{
			RewriteTerminator(block.Terminator, ssaBlock);
		}

		foreach (BasicBlock successor in block.Successors)
		{
			SsaBlock successorSsa = function.Blocks[successor];

			foreach (var phi in successorSsa.Phis)
			{
				var value = _stacks[phi.Variable].Peek();
				phi.Inputs.Add(block, value);
			}
		}

		foreach (BasicBlock child in domTree[block])
		{
			Rename(child, domTree, function);
		}

		RestoreStacks(snapshot);
	}

	private void RewriteTerminator(ControlFlowTerminator terminator, SsaBlock block)
	{
		switch (terminator)
		{
			case ConditionalBranchTerminator condBr:
				SsaValue cond = RewriteExpression(condBr.Condition, block);
				block.Instructions.Add(new CondBrInstruction(cond, condBr.Then, condBr.ElseOrMerged));
				break;
			case ReturnTerminator ret:
				if (ret.Expression != null)
				{
					SsaValue retValue = RewriteExpression(ret.Expression, block);
					block.Instructions.Add(new RetInstruction(retValue));
				}
				else
				{
					block.Instructions.Add(new RetInstruction(null));
				}
				break;
			default:
				throw new UnreachableException($"RewriteTerminator({terminator.GetType()})");
		}
	}

	private void RewriteStatement(BoundStatement statement, SsaBlock block)
	{
		switch (statement)
		{
			case BoundExpressionStatement es:
				RewriteExpression(es.Expression, block);
				break;

			default:
				throw new UnreachableException($"RewriteStatement({statement.GetType()})");
		}
	}

	private SsaValue RewriteExpression(BoundExpression expression, SsaBlock block)
	{
		return expression switch
		{
			BoundLiteral literal => EmitLiteral(literal, block),
			BoundUnaryExpression unary => EmitUnaryExpression(unary, block),
			BoundBinaryExpression binary => EmitBinaryExpression(binary, block),

			_ => throw new UnreachableException($"RewriteExpression({expression.GetType()})")
		};
	}

	private SsaValue EmitLiteral(BoundLiteral literal, SsaBlock block)
	{
		switch (literal.Type.SpecialType)
		{
			case SpecialType.StdNumericsSInt32:
				SsaTemp output = NewTemp(literal.Type);
				LoadImmInstruction imm = new(output, literal.ConstantValue);
				block.Instructions.Add(imm);
				return output;
			default:
				throw new UnreachableException($"EmitLiteral({literal.GetType()})");
		}
	}

	private SsaValue EmitUnaryExpression(BoundUnaryExpression unary, SsaBlock block)
	{
		if (unary.Op.CorrespondingFunction is not null) // user-defined operator
		{
			throw new NotImplementedException("call instruction is not implemented yet");
		}

		SsaValue value = RewriteExpression(unary.Expression, block);

		if (unary.Op.Kind == UnaryOperatorKind.Plus)
		{
			// +expr is a nope operation, no need to allocate new value
			return value;
		}

		SsaTemp result = NewTemp(unary.Type);
		UnaryInstruction instruction = unary.Op.Kind switch
		{
			UnaryOperatorKind.Negate => new NegInstruction(result, value)
		};
		block.Instructions.Add(instruction);
		return result;
	}

	private SsaValue EmitBinaryExpression(BoundBinaryExpression binary, SsaBlock block)
	{
		if (binary.Op.CorrespondingFunction is not null) // user-defined operator
		{
			throw new NotImplementedException("call instruction is not implemented yet");
		}

		SsaValue lhs = RewriteExpression(binary.Left, block);
		SsaValue rhs = RewriteExpression(binary.Right, block);
		SsaTemp result = NewTemp(binary.Type);

		BinaryInstruction instruction = binary.Op.Kind switch
		{
			BinaryOperatorKind.Addition => new AddInstruction(result, lhs, rhs),
			BinaryOperatorKind.Equal => new CmpEqInstruction(result, lhs, rhs),
			_ => throw new UnreachableException($"EmitBinaryExpression({binary.Op.Kind})")
		};

		block.Instructions.Add(instruction);
		return result;
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
}