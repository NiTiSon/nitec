using System.Collections.Immutable;

namespace NiteCompiler.CodeAnalysis.Symbols;

[ForRemoval("Temporal solution, in future will be replaced with standard type resolving phase.")]
public sealed class BuiltinTypeSymbol : TypeSymbol
{
	public override Symbol? ContainingSymbol => null;

	public override string Name { get; }
	public override SpecialType SpecialType { get; }

	public BuiltinTypeSymbol(string name, SpecialType type)
	{
		Name = name;
		SpecialType = type;
	}

	public static BuiltinTypeSymbol I32 = new BuiltinTypeSymbol("SInt32", SpecialType.StdNumericsSInt32);

	public override ImmutableArray<Symbol> GetMembers()
	{
		return [];
	}

	public override void Accept(SymbolVisitor visitor)
	{

	}

	public override TResult? Accept<TResult>(SymbolVisitor<TResult> visitor) where TResult : default
	{
		return default;
	}

	public override TResult? Accept<TResult, TArgument>(SymbolVisitor<TResult, TArgument> visitor, TArgument argument) where TResult : default
	{
		return default;
	}
}