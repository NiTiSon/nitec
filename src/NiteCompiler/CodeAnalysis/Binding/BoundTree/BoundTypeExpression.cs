using NiteCompiler.CodeAnalysis.Binding.Pure;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundTypeExpression : BoundExpression
{
	public override TypeSymbol Type { get; }

	public override Pureness Pureness => Pureness.Pure;

	public override Binder.BindValueKind ValueKind => 0;
	public override BoundKind Kind => BoundKind.TypeExpression;

	public BoundTypeExpression(SyntaxNode syntax, TypeSymbol type, bool hasErrors = false) : base(syntax, hasErrors)
	{
		Type = type;
	}

	public override void Accept(BoundVisitor visitor)
	{
		visitor.VisitType(this);
	}
}