using System;
using System.Collections.Generic;
using System.Linq;
using NiteCompiler.CodeAnalysis.Binding;
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

		Rename(cfg.Entry, ssa);

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

	private void InsertPhiNodes(ControlFlowGraph cfg, Dictionary<LocalVariableOrParameterSymbol, HashSet<BasicBlock>> defs)
	{
	}

	private void Rename(BasicBlock cfgEntry, SsaFunction function)
	{
	}
}