namespace NiteCompiler.CodeAnalysis.Syntax;

public abstract class NameSyntax : TypeSyntax
{
	public virtual string GetName()
	{
		return SyntaxTree.Text.GetText(Span);
	}
}