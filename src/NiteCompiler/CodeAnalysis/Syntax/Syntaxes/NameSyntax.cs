namespace NiteCompiler.CodeAnalysis.Syntax;

public abstract class NameSyntax : TypeSyntax
{
	private protected NameSyntax(SyntaxTree tree) : base(tree) {}

	public abstract string GetName();

	public int Arity => this is GenericNameSyntax
		? ((GenericNameSyntax)this).GenericParameterList.Parameters.Count
		: 0;


	public abstract SimpleNameSyntax UnqualifiedName { get; }
}