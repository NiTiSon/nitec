using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundExpressionStatement : BoundStatement
{
	public BoundExpression Expression { get; }
	public override BoundKind Kind => BoundKind.ExpressionStatement;

	public BoundExpressionStatement(SyntaxNode syntax, BoundExpression expression) : base(syntax)
	{
		Expression = expression;
	}

	public override void Accept(BoundVisitor visitor)
	{
		visitor.VisitExpressionStatement(this);
	}
}