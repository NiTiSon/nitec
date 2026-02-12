namespace NiteCompiler.CodeAnalysis.Syntax;

public abstract class TypeSyntax : ExpressionSyntax
{
	protected TypeSyntax(SyntaxTree tree) : base(tree)
	{
	}
}