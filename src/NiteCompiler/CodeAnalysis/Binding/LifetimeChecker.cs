using System.Collections.Generic;
using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.Diagnostics;
using NiteCompiler.IntermediateRepresentation.ControlFlow;

namespace NiteCompiler.CodeAnalysis.Binding;


internal sealed class LifetimeChecker
{
	private readonly FunctionSymbol _function;
	private readonly BoundBlock _body;
	private readonly ControlFlowGraph _cfg;
	private readonly BindingDiagnosticBag _diagnostics;

	private readonly Dictionary<LocalVariableOrParameterSymbol, HashSet<BasicBlock>> _regions = new();

	private readonly Dictionary<LocalVariableOrParameterSymbol, HashSet<BasicBlock>> _storageLifetimes = new();

	private LifetimeChecker(FunctionSymbol function, BoundBlock body, ControlFlowGraph cfg, BindingDiagnosticBag diagnostics)
	{
		_function = function;
		_body = body;
		_cfg = cfg;
		_diagnostics = diagnostics;
	}

	public static void Check(
		FunctionSymbol function,
		BoundBlock body,
		ControlFlowGraph cfg,
		BindingDiagnosticBag diagnostics)
	{
		var checker = new LifetimeChecker(function, body, cfg, diagnostics);
		checker.Run();
	}

	private void Run()
	{
		ComputeStorageLifetimes();
		ComputeReferenceRegions();
		CheckEscape();
	}

	private void ComputeStorageLifetimes()
	{
		// Parameters are alive for the entire function.
		HashSet<BasicBlock> allBlocks = new(_cfg.Blocks);
		foreach (ParameterSymbol p in _function.Parameters)
		{
			_storageLifetimes[p] = allBlocks;
		}

		// Locals: alive from their declaration block through
		// all blocks reachable before leaving their scope.
		// For now, conservatively mark from definition to function exit.
		// A future improvement: use scope-based pruning from the syntax tree.
		foreach (BasicBlock block in _cfg.Blocks)
		{
			foreach (BoundStatement stmt in block.Statements)
			{
				if (stmt is BoundLocalVariableDeclarationStatement decl)
				{
					// Compute all blocks reachable from this one
					// that are still in the same scope (approximation: all successors).
					var live = ComputeReachableFrom(block);
					_storageLifetimes[decl.Local] = live;
				}
			}
		}
	}

	private HashSet<BasicBlock> ComputeReachableFrom(BasicBlock start)
	{
		var visited = new HashSet<BasicBlock>();
		var queue = new Queue<BasicBlock>();
		queue.Enqueue(start);
		while (queue.Count > 0)
		{
			var b = queue.Dequeue();
			if (!visited.Add(b)) continue;
			foreach (var s in b.Successors) queue.Enqueue(s);
		}
		return visited;
	}

	private void ComputeReferenceRegions()
	{
		// Collect defs: block where each ref-typed variable is first assigned.
		var defBlocks = new Dictionary<LocalVariableOrParameterSymbol, BasicBlock>();
		// Collect uses: all blocks where each variable is read.
		var useBlocks = new Dictionary<LocalVariableOrParameterSymbol, HashSet<BasicBlock>>();

		foreach (BasicBlock block in _cfg.Blocks)
		{
			foreach (BoundStatement stmt in block.Statements)
			{
				CollectDefsAndUses(stmt, block, defBlocks, useBlocks);
			}
			// Also check terminators (return expressions, conditions).
			if (block.Terminator is ReturnTerminator { Expression: { } ret })
				CollectUses(ret, block, useBlocks);
			if (block.Terminator is ConditionalBranchTerminator cond)
				CollectUses(cond.Condition, block, useBlocks);
		}

		// For each ref-typed variable, region = def block ∪ all blocks
		// between def and last use (via liveness backwards traversal).
		foreach (var (sym, defBlock) in defBlocks)
		{
			if (sym.Type is not BaseReferenceTypeSymbol) continue;
			var region = new HashSet<BasicBlock> { defBlock };
			if (useBlocks.TryGetValue(sym, out var uses))
			{
				foreach (BasicBlock useBlock in uses)
					PropagateBackwards(useBlock, defBlock, region);
			}
			_regions[sym] = region;
		}
	}

	private void PropagateBackwards(
		BasicBlock from, BasicBlock stopAt, HashSet<BasicBlock> visited)
	{
		if (!visited.Add(from)) return;
		if (ReferenceEquals(from, stopAt)) return;
		foreach (BasicBlock pred in from.Predecessors)
			PropagateBackwards(pred, stopAt, visited);
	}

	private void CheckEscape()
	{
		foreach (BasicBlock block in _cfg.Blocks)
		{
        	foreach (BoundStatement stmt in block.Statements)
        	{
        	    CheckEscapeInStatement(stmt, block);
        	}
        	if (block.Terminator is ReturnTerminator { Expression: { } retExpr })
        	    CheckReturnEscape(retExpr);
		}
	}

	private void CheckEscapeInStatement(BoundStatement stmt, BasicBlock block)
	{
		switch (stmt)
		{
			case BoundLocalVariableDeclarationStatement decl:
				CheckEscapeInDeclaration(decl);
				break;
			case BoundExpressionStatement { Expression: BoundAssignment assignment }:
				CheckEscapeInAssignment(assignment);
				break;
		}
	}

	private static LocalVariableOrParameterSymbol? ExtractVariable(BoundExpression expr)
	{
		return expr switch
		{
			BoundMove m => m.Variable,
			BoundCopy c => c.Variable,
			BoundLocalVariable l => l.Variable,
			BoundParameter p => p.Variable,
			_ => null
		};
	}

	private void CheckEscapeInDeclaration(BoundLocalVariableDeclarationStatement decl)
	{
		if (decl.Initializer is not BoundAddressOfExpression addrOf) return;

		LocalVariableOrParameterSymbol? referent = ExtractVariable(addrOf.Expression);
		if (referent == null) return;

		LocalVariableOrParameterSymbol refVar = decl.Local;

		if (!_regions.TryGetValue(refVar, out var refRegion)) return;
		if (!_storageLifetimes.TryGetValue(referent, out var referentLife)) return;

		foreach (BasicBlock liveBlock in refRegion)
		{
			if (!referentLife.Contains(liveBlock))
			{
				_diagnostics.Diagnostics.ReportDanglingReference(
					decl.Syntax?.Location ?? addrOf.Syntax!.Location,
					refVar,
					referent);
				break;
			}
		}
	}

	private void CheckEscapeInAssignment(BoundAssignment assignment)
	{
		if (assignment.Right is not BoundAddressOfExpression addrOf) return;

		LocalVariableOrParameterSymbol? referent = ExtractVariable(addrOf.Expression);
		if (referent == null) return;

		LocalVariableOrParameterSymbol? lhsSym = ExtractVariable(assignment.Left);
		if (lhsSym?.Type is not BaseReferenceTypeSymbol) return;
		if (lhsSym == null) return;

		// Assigning a reference to a local into a parameter is always
		// a dangling reference — the parameter's referent must outlive
		// the function call.
		if (lhsSym is ParameterSymbol)
		{
			_diagnostics.Diagnostics.ReportDanglingReference(
				assignment.Syntax?.Location ?? addrOf.Syntax!.Location,
				lhsSym,
				referent);
			return;
		}

		if (!_regions.TryGetValue(lhsSym, out var refRegion)) return;
		if (!_storageLifetimes.TryGetValue(referent, out var referentLife)) return;

		foreach (BasicBlock liveBlock in refRegion)
		{
			if (!referentLife.Contains(liveBlock))
			{
				_diagnostics.Diagnostics.ReportDanglingReference(
					assignment.Syntax?.Location ?? addrOf.Syntax!.Location,
					lhsSym,
					referent);
				break;
			}
		}
	}

	private void CheckReturnEscape(BoundExpression expr)
	{
		LocalVariableOrParameterSymbol? variable = ExtractVariable(expr);
		if (variable == null) return;
		if (variable.Type is not BaseReferenceTypeSymbol) return;

		// Returning a reference with 'static lifetime is always safe —
		// the referent lives for the entire program.
		if (variable.Type is ReferenceTypeSymbol { Lifetime.IsStaticLifetime: true }) return;

		_diagnostics.Diagnostics.ReportDanglingReference(
			expr.Syntax!.Location, variable, variable);
	}

	private static void CollectDefsAndUses(
		BoundStatement stmt,
		BasicBlock block,
		Dictionary<LocalVariableOrParameterSymbol, BasicBlock> defs,
		Dictionary<LocalVariableOrParameterSymbol, HashSet<BasicBlock>> uses)
	{
		if (stmt is BoundLocalVariableDeclarationStatement decl)
		{
			defs.TryAdd(decl.Local, block);
			if (decl.Initializer != null)
				CollectUses(decl.Initializer, block, uses);
		}
		else if (stmt is BoundExpressionStatement es)
		{
			CollectUses(es.Expression, block, uses);
		}
	}

	private static void CollectUses(
		BoundExpression expr,
		BasicBlock block,
		Dictionary<LocalVariableOrParameterSymbol, HashSet<BasicBlock>> uses)
	{
		switch (expr)
		{
			case BoundMove m:
				uses.GetOrAdd(m.Variable, () => new()).Add(block);
				break;
			case BoundCopy c:
				uses.GetOrAdd(c.Variable, () => new()).Add(block);
				break;
			case BoundLocalVariable l:
				uses.GetOrAdd(l.Variable, () => new()).Add(block);
				break;
			case BoundParameter p:
				uses.GetOrAdd(p.Variable, () => new()).Add(block);
				break;
			case BoundAddressOfExpression a:
				CollectUses(a.Expression, block, uses);
				break;
			case BoundDereferenceExpression d:
				CollectUses(d.Expression, block, uses);
				break;
			case BoundAssignment a:
				CollectUses(a.Left, block, uses);
				CollectUses(a.Right, block, uses);
				break;
			case BoundBinaryExpression b:
				CollectUses(b.Left, block, uses);
				CollectUses(b.Right, block, uses);
				break;
			case BoundUnaryExpression u:
				CollectUses(u.Expression, block, uses);
				break;
			case BoundCall c:
				foreach (var arg in c.Arguments)
				{
					CollectUses(arg, block, uses);
				}
				break;
		}
	}
}