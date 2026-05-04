using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class SyntaxList<TNode> : SyntaxNode, IEnumerable<TNode>
	where TNode : SyntaxNode
{
	private readonly TNode[] _nodes;
	public override TextSpan Span => _nodes.Length > 0 ? TextSpan.FromBounds(_nodes[0].Span.Start, _nodes[^1].Span.End) : default;
	public override NodeKind Kind => NodeKind.SyntaxList;
	public int Count => _nodes.Length;
	public bool IsEmpty => Count == 0;

	public TNode this[Index index] => _nodes[index];

	private SyntaxList(SyntaxTree tree, TNode[] nodes) : base(tree)
	{
		_nodes = nodes;
	}

	internal SyntaxList(SyntaxTree tree) : base(tree)
	{
		_nodes = [];
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitSyntaxList(this);
	public override void Accept(SyntaxVisitor visitor) => visitor.VisitSyntaxList(this);

	public static SyntaxList<TNode> CastUp<TDerived>(SyntaxList<TDerived> items)
		where TDerived : TNode
	{
		return new SyntaxList<TNode>(items.Tree, items._nodes.ToArray<TNode>());
	}

	public SyntaxList<TDerived> CastUp<TDerived>()
		where TDerived : TNode
	{
		return new SyntaxList<TDerived>(this.Tree, (TDerived[])this._nodes);
	}

	public static SyntaxList<TNode> GetEmpty(SyntaxTree tree)
	{
		return tree.GetEmptySyntaxList<TNode>();
	}

	public override IEnumerable<TNode> GetChildren()
	{
		return _nodes;
	}

	public IEnumerator<TNode> GetEnumerator()
	{
		return GetChildren().GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	internal sealed class Builder
	{
		private TNode[] _buffer;

		public int Count { get; private set; }
		public int Capacity => _buffer.Length;

		public Builder() : this(8) {}

		public Builder(int initialCapacity)
		{
			_buffer = new TNode[initialCapacity];
		}

		public void Add(TNode node)
		{
			EnsureCapacity(1);
			_buffer[Count++] = node;
		}

		public void AddRange(params ReadOnlySpan<TNode> nodes)
		{
			EnsureCapacity(nodes.Length);
			nodes.CopyTo(_buffer.AsSpan().Slice(Count));
			Count += nodes.Length;
		}

		public void Clear()
		{
			Array.Clear(_buffer, 0, Count);
			Count = 0;
		}

		public void EnsureCapacity(int additionalSize)
		{
			int newSize = Count + additionalSize;

			if (newSize < _buffer.Length) return;

			Array.Resize(ref _buffer, int.Max(Count * 2, newSize));
		}

		public SyntaxList<TNode> Build(SyntaxTree tree)
		{
			int capacity = _buffer.Length;

			if (capacity > Count)
			{
				TNode[] temp = new TNode[Count];
				Array.Copy(_buffer, temp, Count);
				Clear();
				return new SyntaxList<TNode>(tree, temp);
			}
			else // capacity == _count
			{
				TNode[] buffer = _buffer;
				_buffer = new TNode[8];
				return new SyntaxList<TNode>(tree, buffer);
			}
		}
	}
}