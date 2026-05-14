using System.Collections.Immutable;

namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class ContainerSymbol : Symbol
{
	public abstract ImmutableArray<Symbol> GetMembers();

	// TODO:
	// + GetMembers(name)
	// + GetTypeMembers()
	// + GetTypeMembers(name, int?)

	public abstract ImmutableArray<Symbol> GetMembers(string name);
	public abstract ImmutableArray<TypeSymbol> GetTypeMembers();
	public abstract ImmutableArray<TypeSymbol> GetTypeMembers(string name, int? arity);

	public virtual ImmutableArray<ModuleSymbol> GetNestedModules()
	{
		var members = GetMembers();

		if (members.IsDefaultOrEmpty)
		{
			return [];
		}

		var result = ImmutableArray.CreateBuilder<ModuleSymbol>();

		foreach (var member in members)
		{
			if (member.Kind == SymbolKind.Module)
			{
				result.Add((ModuleSymbol)member);
			}
		}

		return result.ToImmutable();
	}

	public virtual ModuleSymbol? GetNestedModule(string name)
	{
		// foreach (var member in GetMembers(name))
		// {
		// 	if (member.Kind == SymbolKind.Module)
		// 	{
		// 		return (ModuleSymbol)member;
		// 	}
		// }
		foreach (var member in GetMembers())
		{
			if (member.Kind == SymbolKind.Module && member.Name == name)
			{
				return (ModuleSymbol)member;
			}
		}

		return null;
	}
}