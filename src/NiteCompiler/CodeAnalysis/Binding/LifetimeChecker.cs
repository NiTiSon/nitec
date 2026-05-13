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
	    if (stmt is not BoundLocalVariableDeclarationStatement decl) return;
	    if (decl.Initializer is not BoundAddressOfExpression addrOf) return;

	    // Find what symbol is being addressed.
	    LocalVariableOrParameterSymbol? referent = addrOf.Expression switch
	    {
	        BoundLocal l => l.Local,
	        BoundParameter p => p.Parameter,
	        _ => null
	    };
	    if (referent == null) return;

	    // The new reference variable.
	    LocalVariableOrParameterSymbol refVar = decl.Local;

	    if (!_regions.TryGetValue(refVar, out var refRegion)) return;
	    if (!_storageLifetimes.TryGetValue(referent, out var referentLife)) return;

	    // Every block where the reference is live must be covered
	    // by the referent's storage lifetime.
	    foreach (BasicBlock liveBlock in refRegion)
	    {
	        if (!referentLife.Contains(liveBlock))
	        {
	            // The reference is used in a block where the
	            // referent is no longer alive → dangling reference.
	            _diagnostics.Diagnostics.ReportDanglingReference(
	                decl.Syntax?.Location ?? addrOf.Syntax!.Location,
	                refVar,
	                referent);
	            break; // one error per variable is enough
	        }
	    }
	}

	private void CheckReturnEscape(BoundExpression expr)
	{
	    // Returning a reference to a local is always an escape.
	    if (expr is not BoundLocal local) return;
	    if (local.Type is not BaseReferenceTypeSymbol) return;

	    // If this local was initialized with &someOtherLocal,
	    // and someOtherLocal is not a parameter, it escapes.
	    // (Full escape analysis requires tracking provenance through assignments;
	    //  this is the conservative first pass.)
	    _diagnostics.Diagnostics.ReportDanglingReference(
	        expr.Syntax!.Location, local.Local, local.Local);
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
			case BoundLocal l:
				uses.GetOrAdd(l.Local, () => new()).Add(block);
				break;
			case BoundParameter p:
				uses.GetOrAdd(p.Parameter, () => new()).Add(block);
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