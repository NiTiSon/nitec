namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public abstract class StatementSyntax : SyntaxNode
{
	private protected StatementSyntax(SyntaxTree tree) : base(tree) { }
}