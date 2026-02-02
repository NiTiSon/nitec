namespace NiteCompiler.CodeAnalysis.Syntax;

public abstract class ExpressionSyntax : SyntaxNode
{
	protected ExpressionSyntax(SyntaxTree tree) : base(tree)
	{
	}
}