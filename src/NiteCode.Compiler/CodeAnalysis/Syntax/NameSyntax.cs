namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public abstract class NameSyntax : ExpressionSyntax
{
	private protected NameSyntax(SyntaxTree tree) : base(tree) { }
}