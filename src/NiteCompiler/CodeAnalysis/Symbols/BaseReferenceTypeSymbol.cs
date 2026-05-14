using System.Collections.Immutable;

namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class BaseReferenceTypeSymbol : TypeSymbol
{
	public sealed override Symbol? ContainingSymbol => null;
	public override LibrarySymbol? ContainingLibrary => null;

	public abstract TypeSymbol PointsTo { get; }
	public virtual bool IsFatPointer => false;
	public abstract bool IsNullable { get; }
	public abstract bool IsMutable { get; }

	public override ImmutableArray<Symbol> GetMembers()
	{
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