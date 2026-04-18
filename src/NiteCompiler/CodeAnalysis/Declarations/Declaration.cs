using System.Collections.Immutable;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Declarations;

internal abstract class Declaration
{
	public string Name { get; }
	public abstract DeclarationKind Kind { get; }
	public ImmutableArray<Declaration> Members => GetDeclarationMembers();

	protected Declaration(string name)
	{
		Name = name;
	}

	protected abstract ImmutableArray<Declaration> GetDeclarationMembers();
}