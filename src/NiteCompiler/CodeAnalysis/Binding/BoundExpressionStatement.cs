using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundExpressionStatement : BoundStatement
{
	public BoundExpression Expression { get; }

	public BoundExpressionStatement(SyntaxNode syntax, BoundExpression expression) : base(syntax)
	{
		Expression = expression;
	}

	public override BoundKind Kind => BoundKind.ExpressionStatement;
}