namespace NiteCompiler.CodeAnalysis.Syntax;

public abstract class ConstraintSyntax : SyntaxNode
{
	private protected ConstraintSyntax(SyntaxTree tree) : base(tree)
	{
	}

	public sealed override void Accept(SyntaxVisitor visitor)
	{
		visitor.VisitConstraint(this);
	}

	public sealed override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default
	{
		return visitor.VisitConstraint(this);
	}
}