using System;
using System.Collections.Generic;
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
		BasicBlock entry = builder.NewBlock();
		builder._current = entry;

		builder.Visit(body);

		BasicBlock exit = builder.NewBlock();

		if (builder._current.Terminator == null)
		{
			Connect(builder._current, exit);
		}

		return new ControlFlowGraph(entry, builder._blocks.ToArray());
	}

	private BasicBlock NewBlock()
	{
		var block = new BasicBlock(_blocks.Count);
		_blocks.Add(block);
		return block;
	}

	private static void Connect(BasicBlock from, BasicBlock to)
	{
		from.Successors.Add(to);
		to.Predecessors.Add(from);

		from.Terminator ??= new BranchTerminator(node: null, to);
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

		_current = NewBlock();
	}

	public override void VisitIfStatement(BoundIfStatement node)
	{
		BasicBlock thenBlock = NewBlock();
		BasicBlock? elseBlock = node.ElseStatement != null ? NewBlock() : null;
		BasicBlock mergeBlock = NewBlock();

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