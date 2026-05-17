using System.Collections.Immutable;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceGenericTypeParameterSymbol : GenericTypeParameterSymbol
{
	public override Symbol ContainingSymbol { get; }
	public override string Name { get; }
	public override int Ordinal { get; }

	public SourceGenericTypeParameterSymbol(Symbol containingSymbol, string name, int ordinal)
	{
		ContainingSymbol = containingSymbol;
		Name = name;
		Ordinal = ordinal;
	}

	public override ImmutableArray<Symbol> GetMembers() => [];

	public override ImmutableArray<Symbol> GetMembers(string name) => [];

	public override ImmutableArray<TypeSymbol> GetTypeMembers() => [];

	public override ImmutableArray<TypeSymbol> GetTypeMembers(string name, int? arity = null) => [];
}
