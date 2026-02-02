namespace NiteCompiler.CodeAnalysis.Syntax;

public abstract class NameSyntax : ExpressionSyntax
{
	protected NameSyntax(SyntaxTree tree) : base(tree) {}

	public abstract string ShortName { get; }

	public virtual string GetFullName() => ShortName;
}