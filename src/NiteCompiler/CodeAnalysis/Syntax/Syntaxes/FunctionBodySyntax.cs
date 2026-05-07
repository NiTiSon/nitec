namespace NiteCompiler.CodeAnalysis.Syntax;

public abstract class FunctionBodySyntax : BodySyntax
{
	internal abstract Token ClosingToken { get; }

	private protected FunctionBodySyntax(SyntaxTree tree) : base(tree)
	{
	}

	public sealed override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default
	{
		return visitor.VisitFunctionBody(this);
	}

	public sealed override void Accept(SyntaxVisitor visitor)
	{
		visitor.VisitFunctionBody(this);
	}
}