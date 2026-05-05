using System.Linq;

namespace NiteCompiler.CodeAnalysis.Syntax;

public abstract class NameSyntax : TypeSyntax
{
	private protected NameSyntax(SyntaxTree tree) : base(tree) {}

	public abstract string GetName();

	public int Arity => this is GenericNameSyntax
		? ((GenericNameSyntax)this).GenericParameterList.Parameters
			.Count(t => t.IsGenericParameter)
		: 0;

	public int LifetimeArity => this is GenericNameSyntax
		? ((GenericNameSyntax)this).GenericParameterList.Parameters
			.Count(t => !t.IsGenericParameter)
		: 0;

	public abstract SimpleNameSyntax UnqualifiedName { get; }
}