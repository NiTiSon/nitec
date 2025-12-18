namespace NiteCompiler.CodeAnalysis.Syntax;

public abstract class UseOrUseAsDirectiveSyntax : SyntaxNode
{
	public bool IsAlias => this is not UseDirectiveSyntax;
}