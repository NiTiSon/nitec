using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundType : BoundNode
{
	public ImmutableArray<BoundNode> Members { get; }

	public BoundType(SyntaxNode syntax, TypeSymbol type, ImmutableArray<BoundNode> members) : base(syntax)
	{
		Members = members;
	}

	public override BoundKind Kind => BoundKind.Type;
}