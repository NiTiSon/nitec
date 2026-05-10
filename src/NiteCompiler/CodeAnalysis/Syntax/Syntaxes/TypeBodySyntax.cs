namespace NiteCompiler.CodeAnalysis.Syntax;

public abstract class TypeBodySyntax : BodySyntax
{
	private protected TypeBodySyntax(SyntaxTree tree) : base(tree)
	{
	}

	public sealed override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default
	{
		return visitor.VisitTypeBody(this);
	}

	public sealed override void Accept(SyntaxVisitor visitor)
	{
		visitor.VisitTypeBody(this);
	}
}