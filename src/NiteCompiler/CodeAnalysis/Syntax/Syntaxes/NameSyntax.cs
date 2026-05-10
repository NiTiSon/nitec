namespace NiteCompiler.CodeAnalysis.Syntax;

public abstract class NameSyntax : TypeSyntax
{
	private protected NameSyntax(SyntaxTree tree) : base(tree) {}

	public abstract string GetName();

	public abstract SimpleNameSyntax UnqualifiedName { get; }

	public virtual int LifetimeArity => 0;
	public virtual int Arity => 0;

	public sealed override void Accept(SyntaxVisitor visitor)
	{
		visitor.VisitName(this);
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default
	{
		return visitor.VisitName(this);
	}
}