namespace NiteCompiler.CodeAnalysis.Syntax;

public abstract class StatementSyntax : SyntaxNode
{
	public virtual bool IsRequiresSemicolon => true;
}