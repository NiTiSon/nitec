using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundUnaryExpression : BoundExpression
{
	public BoundUnaryOperator Operator { get; }
	public BoundExpression Expression { get; }
	public override BoundKind Kind => BoundKind.UnaryExpression;
	public override TypeSymbol Type => Operator.Type;
	public BoundUnaryExpression(SyntaxNode syntax, BoundUnaryOperator op, BoundExpression expression) : base(syntax)
	{
		Operator = op;
		Expression = expression;
	}
}