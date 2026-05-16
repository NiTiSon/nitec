using System.Collections.Immutable;

namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class BaseArrayTypeSymbol : TypeSymbol
{
	public sealed override Symbol? ContainingSymbol => null;
	public override LibrarySymbol? ContainingLibrary => null;

	public abstract TypeSymbol ElementsType { get; }
	public virtual int Rank => 1;
	public abstract int? Length { get; }

	public override ImmutableArray<Symbol> GetMembers()
	{
		// TODO: actually we want array type to have some members: get/set indexation methods, size property, etc.
		// Solution: backing a predefined Array<T> type?
		return [];
	}

	public override ImmutableArray<Symbol> GetMembers(string name)
	{
		return [];
	}

	public override ImmutableArray<TypeSymbol> GetTypeMembers()
	{
		return [];
	}

	public override ImmutableArray<TypeSymbol> GetTypeMembers(string name, int? arity)
	{
		return [];
	}
}