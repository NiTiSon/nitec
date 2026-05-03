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
}