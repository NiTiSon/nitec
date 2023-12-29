namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public abstract class ExpressionSyntax : SyntaxNode
{
	private protected ExpressionSyntax(SyntaxTree tree) : base(tree) { }
}