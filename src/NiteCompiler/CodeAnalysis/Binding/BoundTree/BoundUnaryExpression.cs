using NiteCompiler.CodeAnalysis.Binding.Operators;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundUnaryExpression : BoundExpression
{
	public UnaryOperatorSignature Op { get; }
	public BoundExpression Expression { get; }

	public override BoundKind Kind => BoundKind.UnaryExpression;
	public override Binder.BindValueKind ValueKind => Binder.BindValueKind.RValue;
	public override TypeSymbol Type => Op.ReturnType;

	public BoundUnaryExpression(SyntaxNode syntax, UnaryOperatorSignature op, BoundExpression expression)
		: base(syntax)
	{
		Op = op;
		Expression = expression;
	}

	public override void Accept(BoundVisitor visitor)
	{
		visitor.VisitUnaryExpression(this);
	}
}