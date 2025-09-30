using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class SyntaxList<TNode> : SyntaxNode, IEnumerable<TNode>
	where TNode : SyntaxNode
{
	private readonly ImmutableArray<TNode> _nodes;
	public override TextSpan Span => _nodes.Length > 0 ? TextSpan.FromBounds(_nodes[0].Span.Start, _nodes[^1].Span.End) : default;
	public override SyntaxKind Kind { get; }

	private SyntaxList(ImmutableArray<TNode>.Builder nodes, SyntaxKind listKind)
	{
		_nodes = nodes.DrainToImmutable();
		Kind = listKind;
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

	internal readonly struct Builder
	{
		private readonly SyntaxKind _listKind;
		private readonly ImmutableArray<TNode>.Builder _builder;

		[Obsolete("Use Builder(SyntaxKind) constructor instead.")]
		public Builder()
		{
			throw new("Use Builder(SyntaxKind) constructor instead.");
		}

		public Builder(SyntaxKind listKind)
		{
			_listKind = listKind;
			_builder = ImmutableArray.CreateBuilder<TNode>();
		}

		public void Add(TNode node)
		{
			_builder.Add(node);
		}

		public SyntaxList<TNode> Build()
		{
			return new(_builder, _listKind);
		}

		public void RemoveLast()
		{
			_builder.RemoveAt(_builder.Count - 1);
		}
	}
}