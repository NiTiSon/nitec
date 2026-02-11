namespace NiteCompiler.CodeAnalysis.Syntax;

public abstract class TopLevelSyntax : SyntaxNode
{
	protected TopLevelSyntax(SyntaxTree tree) : base(tree) {}
}