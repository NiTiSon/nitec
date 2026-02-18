using System.Collections.Immutable;

namespace NiteCompiler.CodeAnalysis.Symbols.Declarations;

internal abstract class Declaration
{
	public string Name { get; }
	public abstract DeclarationKind Kind { get; }
	public ImmutableArray<Declaration> Children => GetDeclarationChildren();

	protected Declaration(string name)
	{
		Name = name;
	}

	protected abstract ImmutableArray<Declaration> GetDeclarationChildren();
}

internal class ModuleDeclaration : Declaration
{
	public ModuleDeclaration(string name, ImmutableArray<Declaration> members)
	{
	}
}