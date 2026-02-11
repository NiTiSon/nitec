namespace NiteCompiler.CodeAnalysis.Syntax;

public abstract class StatementSyntax : SyntaxNode
{
	protected StatementSyntax(SyntaxTree tree) : base(tree) {}
}