namespace NiteCompiler.CodeAnalysis.Syntax;

public abstract class ItemSyntax : SyntaxNode
{
	private protected ItemSyntax(SyntaxTree tree) : base(tree)
	{
	}
}