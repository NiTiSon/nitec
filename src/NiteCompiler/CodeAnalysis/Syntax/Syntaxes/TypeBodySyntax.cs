namespace NiteCompiler.CodeAnalysis.Syntax;

public abstract class TypeBodySyntax : BodySyntax
{
	private protected TypeBodySyntax(SyntaxTree tree) : base(tree)
	{
	}

	internal abstract Token ClosingToken { get; }
}