using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Binding.Pure;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundCollectionExpression : BoundExpression
{
	public ImmutableArray<BoundExpression> Elements { get; }

	public override BoundKind Kind => BoundKind.CollectionExpression;
	public override TypeSymbol Type { get; }
	public override Binder.BindValueKind ValueKind => Binder.BindValueKind.RValue;
	public override Pureness Pureness => Pureness.Pure;

	public BoundCollectionExpression(SyntaxNode syntax, ImmutableArray<BoundExpression> elements, TypeSymbol type, bool hasErrors = false) : base(syntax, hasErrors)
	{
		Elements = elements;
		Type = type;
	}

	public override void Accept(BoundVisitor visitor)
	{
		visitor.VisitCollectionExpression(this);
	}
}
