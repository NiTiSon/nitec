namespace NiteCompiler.CodeAnalysis.Syntax;

public abstract class GenericOrLifetimeParameterSyntax : SyntaxNode
{
	private protected GenericOrLifetimeParameterSyntax(SyntaxTree tree) : base(tree)
	{
	}

	public bool IsGenericParameter => this is not LifetimeParameterSyntax;
}