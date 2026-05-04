using NiteCompiler.CodeAnalysis.Binding.Pure;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundDereferenceExpression : BoundExpression
{
	public BoundExpression Expression { get; }
	public override BoundKind Kind => BoundKind.DereferenceExpression;
	public override TypeSymbol Type { get; }
	public override Pureness Pureness => Expression.Pureness;
	public override Binder.BindValueKind ValueKind { get; }

	public BoundDereferenceExpression(SyntaxNode syntax, BoundExpression expression, TypeSymbol type, bool isMutable, bool hasErrors = false)
		: base(syntax, hasErrors || expression.HasErrors)
	{
		Expression = expression;
		Type = type;
		ValueKind = isMutable
			? Binder.BindValueKind.LValue | Binder.BindValueKind.RValue
			: Binder.BindValueKind.RValue;
	}

	public override void Accept(BoundVisitor visitor)
	{
		visitor.VisitDereferenceExpression(this);
	}
}
