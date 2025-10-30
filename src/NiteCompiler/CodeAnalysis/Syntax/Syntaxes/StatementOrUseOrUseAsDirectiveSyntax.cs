namespace NiteCompiler.CodeAnalysis.Syntax;

public abstract class StatementOrUseOrUseAsDirectiveSyntax : SyntaxNode
{
	public virtual bool IsRequiresSemicolon => true;
}