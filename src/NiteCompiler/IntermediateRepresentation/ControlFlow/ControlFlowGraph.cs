using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace NiteCompiler.IntermediateRepresentation.ControlFlow;

internal sealed class ControlFlowGraph
{
	public BasicBlock[] Blocks { get; }
	public BasicBlock Entry { get; }

	public ControlFlowGraph(BasicBlock entry, BasicBlock[] blocks)
	{
		Debug.Assert(blocks.Contains(entry));

		Blocks = blocks;
		Entry = entry;
	}

	public IReadOnlyDictionary<BasicBlock, HashSet<BasicBlock>> Dominators
	{
		get
		{
			if (field == null)
			{
				Interlocked.CompareExchange(ref field, ComputeDominators(this), null);
			}

			return field;
		}
	}

	public IReadOnlyDictionary<BasicBlock, BasicBlock?> ImmediateDominators
	{
		get
		{
			if (field == null)
			{
				Interlocked.CompareExchange(ref field, ComputeImmediateDominators(this, this.Dominators), null);
			}

			return field;
		}
	}

	public IReadOnlyDictionary<BasicBlock, List<BasicBlock>> DominatorTree
	{
		get
		{
			if (field == null)
			{
				Interlocked.CompareExchange(ref field, ComputeDominatorTree(this.ImmediateDominators), null);
			}

			return field;
		}
	}

	public IReadOnlyDictionary<BasicBlock, HashSet<BasicBlock>> DominanceFrontier
	{
		get
		{
			if (field == null)
			{
				Interlocked.CompareExchange(ref field, ComputeDominanceFrontier(this, ImmediateDominators), null);
			}

			return field;
		}
	}

	public bool Dominates(BasicBlock a, BasicBlock b)
	{
		Debug.Assert(Blocks.Contains(a));
		Debug.Assert(Blocks.Contains(b));
		return Dominators[b].Contains(a);
	}

	private static Dictionary<BasicBlock, HashSet<BasicBlock>> ComputeDominators(ControlFlowGraph cfg)
	{
		BasicBlock[] blocks = cfg.Blocks;
		Dictionary<BasicBlock, HashSet<BasicBlock>> dom = new();

		// Initialize
		foreach (var b in blocks)
		{
			dom[b] = new HashSet<BasicBlock>(blocks);
		}

		// Entry dominates only itself
		dom[cfg.Entry] = [cfg.Entry];

		bool changed;
		do
		{
			changed = false;

			foreach (BasicBlock block in blocks)
			{
				if (ReferenceEquals(block, cfg.Entry))
					continue;

				// Intersect dominators of all predecessors
				HashSet<BasicBlock>? newDom = null;

				foreach (var p in block.Predecessors)
				{
					if (newDom == null)
						newDom = [..dom[p]];
					else
						newDom.IntersectWith(dom[p]);
				}

				// Add self
				newDom ??= [];
				newDom.Add(block);

				if (!dom[block].SetEquals(newDom))
				{
					dom[block] = newDom;
					changed = true;
				}
			}

		} while (changed);

		return dom;
	}

	private static Dictionary<BasicBlock, BasicBlock?> ComputeImmediateDominators(ControlFlowGraph cfg,
		IReadOnlyDictionary<BasicBlock, HashSet<BasicBlock>> dom)
	{
		Dictionary<BasicBlock, BasicBlock?> idom = [];

		foreach (BasicBlock b in cfg.Blocks)
		{
			if (ReferenceEquals(b, cfg.Entry))
			{
				idom[b] = null; // entry has no idom
				continue;
			}

			// strict dominators = dom[b] - {b}
			HashSet<BasicBlock> strictDominators = new(dom[b]);
			strictDominators.Remove(b);

			BasicBlock? immediate = null;

			foreach (BasicBlock d in strictDominators)
			{
				bool isImmediate = true;

				foreach (BasicBlock other in strictDominators)
				{
					if (other == d)
						continue;

					// if d is dominated by another dominator → not immediate
					if (dom[other].Contains(d))
					{
						isImmediate = false;
						break;
					}
				}

				if (isImmediate)
				{
					immediate = d;
					break;
				}
			}

			idom[b] = immediate;
		}

		return idom;
	}

	private static Dictionary<BasicBlock, List<BasicBlock>> ComputeDominatorTree(IReadOnlyDictionary<BasicBlock, BasicBlock?> idom)
	{
		var tree = new Dictionary<BasicBlock, List<BasicBlock>>();

		foreach (BasicBlock b in idom.Keys)
		{
			tree[b] = [];
		}

		foreach (var (block, parent) in idom)
		{
			if (parent != null)
			{
				tree[parent].Add(block);
			}
		}

		return tree;
	}

	private static Dictionary<BasicBlock, HashSet<BasicBlock>> ComputeDominanceFrontier(ControlFlowGraph cfg, IReadOnlyDictionary<BasicBlock, BasicBlock?> idom)
	{
		Dictionary<BasicBlock, HashSet<BasicBlock>> df = [];

		foreach (BasicBlock b in cfg.Blocks)
		{
			df[b] = [];
		}

		foreach (BasicBlock b in cfg.Blocks)
		{
			if (b.Predecessors.Count < 2)
				continue;

			foreach (BasicBlock p in b.Predecessors)
			{
				BasicBlock? runner = p;

				while (runner != null && !ReferenceEquals(runner, idom[b]))
				{
					df[runner].Add(b);
					runner = idom[runner];
				}
			}
		}

		return df;
	}
}
