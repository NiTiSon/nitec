namespace NiteCompiler.CodeAnalysis.Syntax;

public abstract class GenericParameterSyntax : LifetimeOrGenericParameterSyntax
{
	private protected GenericParameterSyntax(SyntaxTree tree) : base(tree)
	{

	}

	public sealed override void Accept(SyntaxVisitor visitor)
	{
		visitor.VisitGenericParameter(this);
	}

	public sealed override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default
	{
		return visitor.VisitGenericParameter(this);
	}
}