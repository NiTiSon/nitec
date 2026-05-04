using NiteCompiler.CodeAnalysis.Binding.Pure;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundAddressOfExpression : BoundExpression
{
	public BoundExpression Expression { get; }
	public override BoundKind Kind => BoundKind.AddressOfExpression;
	public override TypeSymbol Type { get; }
	public override Pureness Pureness => Expression.Pureness;
	public override Binder.BindValueKind ValueKind => Binder.BindValueKind.RValue;

	public BoundAddressOfExpression(SyntaxNode syntax, BoundExpression expression, TypeSymbol type, bool hasErrors = false)
		: base(syntax, hasErrors || expression.HasErrors)
	{
		Expression = expression;
		Type = type;
	}

	public override void Accept(BoundVisitor visitor)
	{
		visitor.VisitAddressOfExpression(this);
	}
}
