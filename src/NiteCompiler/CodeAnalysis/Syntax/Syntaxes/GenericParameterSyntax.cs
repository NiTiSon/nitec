namespace NiteCompiler.CodeAnalysis.Syntax;

public abstract class GenericParameterSyntax : GenericOrLifetimeParameterSyntax
{
	private protected GenericParameterSyntax(SyntaxTree tree) : base(tree)
	{
	}
}