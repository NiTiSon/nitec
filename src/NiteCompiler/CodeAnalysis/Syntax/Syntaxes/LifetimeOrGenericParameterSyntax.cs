namespace NiteCompiler.CodeAnalysis.Syntax;

public abstract class LifetimeOrGenericParameterSyntax : SyntaxNode
{
	private protected LifetimeOrGenericParameterSyntax(SyntaxTree tree) : base(tree)
	{
	}

	public bool IsGenericParameter => this is not LifetimeSyntax;
}