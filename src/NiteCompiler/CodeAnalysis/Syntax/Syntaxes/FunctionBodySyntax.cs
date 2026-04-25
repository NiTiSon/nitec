namespace NiteCompiler.CodeAnalysis.Syntax;

public abstract class FunctionBodySyntax : BodySyntax
{
	private protected FunctionBodySyntax(SyntaxTree tree) : base(tree)
	{
	}

	internal abstract Token ClosingToken { get; }
}