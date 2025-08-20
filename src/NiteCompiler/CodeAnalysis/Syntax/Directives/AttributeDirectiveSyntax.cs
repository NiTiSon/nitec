namespace NiteCompiler.CodeAnalysis.Syntax.Directives;

/// <summary>
/// Base class for all directives like <c>#when(...)</c>
/// </summary>
public abstract class AttributeDirectiveSyntax : SyntaxNode
{
	public Token Hash { get; }

	protected AttributeDirectiveSyntax(Token hash)
	{
		Hash = hash;
	}
}