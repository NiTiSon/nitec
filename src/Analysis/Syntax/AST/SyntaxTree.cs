namespace NiteCompiler.Analysis.Syntax.AST;

public abstract class SyntaxTree : SyntaxNode
{
	protected SyntaxTree(SyntaxKind kind) : base(kind)
	{
	}
}
