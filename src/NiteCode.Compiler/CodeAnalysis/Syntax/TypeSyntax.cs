namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public abstract class TypeSyntax : ExpressionSyntax
{
	private protected TypeSyntax(SyntaxTree tree) : base(tree) { }
}