using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NiteCompiler.CodeAnalysis.Binding;

namespace NiteCompiler.IntermediateRepresentation.ControlFlow;

internal sealed class ControlFlowGraphBuilder : BoundVisitor
{
	private readonly List<BasicBlock> _blocks = [];
	private BasicBlock _current = null!;

	private ControlFlowGraphBuilder() { }

	public static ControlFlowGraph Build(BoundBlock body)
	{
		ControlFlowGraphBuilder builder = new();
		BasicBlock entry = builder.NewBlock("entry");
		builder._current = entry;

		builder.Visit(body);

		Debug.Assert(builder._current.Terminator == null);

		builder.RemoveUnreachableBlocks(entry);
		return new ControlFlowGraph(entry, builder._blocks.ToArray());
	}

	private BasicBlock NewBlock(string? name = null)
	{
		name ??= "";
		name += _blocks.Count;
		var block = new BasicBlock(name);
		_blocks.Add(block);
		return block;
	}

	private static void Connect(BasicBlock from, BasicBlock to)
	{
		from.Successors.Add(to);
		to.Predecessors.Add(from);

		from.Terminator ??= new BranchTerminator(node: null, to);
	}

	private static HashSet<BasicBlock> ComputeReachable(BasicBlock entry)
	{
		var visited = new HashSet<BasicBlock>();
		var stack = new Stack<BasicBlock>();

		stack.Push(entry);

		while (stack.Count > 0)
		{
			var block = stack.Pop();

			if (!visited.Add(block))
				continue;

			foreach (var succ in block.Successors)
				stack.Push(succ);
		}

		return visited;
	}

	private void RemoveUnreachableBlocks(BasicBlock entry)
	{
		HashSet<BasicBlock> reachable = ComputeReachable(entry);

		for (int i = _blocks.Count - 1; i >= 0; i--)
		{
			BasicBlock block = _blocks[i];

			if (reachable.Contains(block)) continue;

			foreach (BasicBlock predecessor in block.Predecessors)
			{
				predecessor.Successors.Remove(block);
			}

			foreach (BasicBlock successor in block.Successors)
			{
				successor.Predecessors.Remove(block);
			}

			_blocks.RemoveAt(i);
		}
	}

	public override void VisitExpressionStatement(BoundExpressionStatement statement)
	{
		_current.Statements.Add(statement);
	}

	public override void VisitBlock(BoundBlock node)
	{
		foreach (var statement in node.Statements)
		{
			Visit(statement);
		}
	}

	public override void VisitReturn(BoundReturn returnStatement)
	{
		_current.Terminator = new ReturnTerminator(returnStatement);

		_current = NewBlock("return.after");
	}

	public override void VisitIfStatement(BoundIfStatement node)
	{
		BasicBlock thenBlock = NewBlock("if.then");
		BasicBlock? elseBlock = node.ElseStatement != null ? NewBlock("if.else") : null;
		BasicBlock mergeBlock = NewBlock("if.merge");

		BasicBlock entry = _current;

		Connect(_current, thenBlock);
		Connect(_current, elseBlock ?? mergeBlock);

		_current = thenBlock;
		Visit(node.ThenStatement);
		if (_current.Terminator == null)
		{
			Connect(_current, mergeBlock);
		}

		if (elseBlock != null)
		{
			_current = elseBlock;
			Visit(node.ElseStatement);
			if (_current.Terminator == null)
			{
				Connect(_current, mergeBlock);
			}
		}

		entry.Terminator = new ConditionalBranchTerminator(node, thenBlock, elseOrMerged: elseBlock ?? mergeBlock);

		_current = mergeBlock;
	}
}