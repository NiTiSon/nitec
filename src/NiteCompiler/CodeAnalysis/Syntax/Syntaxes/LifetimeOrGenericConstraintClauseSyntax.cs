namespace NiteCompiler.CodeAnalysis.Syntax;

public abstract class LifetimeOrGenericConstraintClauseSyntax : SyntaxNode
{
	private protected LifetimeOrGenericConstraintClauseSyntax(SyntaxTree tree) : base(tree)
	{
	}

	public sealed override void Accept(SyntaxVisitor visitor)
	{
		visitor.VisitLifetimeOrGenericConstraintClause(this);
	}

	public sealed override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default
	{
		return visitor.VisitLifetimeOrGenericConstraintClause(this);
	}
}