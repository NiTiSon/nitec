namespace NiteCompiler.Analysis.Syntax;

public abstract class SyntaxNode
{
	public readonly SyntaxKind Kind;

	protected SyntaxNode(SyntaxKind kind)
	{
		Kind = kind;
	}
}
