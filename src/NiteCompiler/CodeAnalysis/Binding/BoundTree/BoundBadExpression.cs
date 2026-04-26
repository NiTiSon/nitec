using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Binding.Pure;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundBadExpression : BoundExpression
{
	public LookupResultKind ResultKind { get; }
	public ImmutableArray<Symbol> Symbols { get; }
	public ImmutableArray<BoundExpression> ChildBoundNodes { get; }

	public override BoundKind Kind => BoundKind.BadExpression;
	public override Binder.BindValueKind ValueKind => Binder.BindValueKind.RValue;
	public override Pureness Pureness => Pureness.None;
	public override TypeSymbol Type { get; }

	public BoundBadExpression(SyntaxNode syntax, LookupResultKind resultKind, ImmutableArray<Symbol> symbols, ImmutableArray<BoundExpression> childBoundNodes, TypeSymbol type)
		: base(syntax, hasErrors: true)
	{
		ResultKind = resultKind;
		Symbols = symbols;
		ChildBoundNodes = childBoundNodes;
		Type = type;
	}

	public override void Accept(BoundVisitor visitor)
	{
		visitor.VisitBadExpression(this);
	}
}