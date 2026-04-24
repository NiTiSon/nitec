namespace NiteCompiler.CodeAnalysis.Syntax;

public abstract class NameSyntax : TypeSyntax
{
	protected NameSyntax(SyntaxTree tree) : base(tree) {}

	public abstract string GetName();

	public int Arity
	{
		get
		{
			return 0;
			//return this is GenericNameSyntax ? ((GenericNameSyntax)this).TypeArgumentList.Arguments.Count : 0;
		}
	}


	public abstract SimpleNameSyntax UnqualifiedName { get; }
}