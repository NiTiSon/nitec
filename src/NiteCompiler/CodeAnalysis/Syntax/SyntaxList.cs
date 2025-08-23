using System.Collections.Generic;
using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class SyntaxList<TNode> : SyntaxNode
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

	internal struct Builder
	{
		private readonly SyntaxKind _listKind;
		private ImmutableArray<TNode>.Builder _builder;

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
	}
}