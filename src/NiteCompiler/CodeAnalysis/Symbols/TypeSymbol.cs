using System;
using System.Collections.Generic;

namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class TypeSymbol : Symbol, IContainerSymbol, IMemberSymbol, IEquatable<TypeSymbol>
{
	public sealed override SymbolKind Kind => SymbolKind.Type;
	public abstract IEnumerable<IMemberSymbol> Members { get; }
	public abstract IContainerSymbol? ContainingSymbol { get; }
	public abstract TypeSymbol? Parent { get; }

	private protected TypeSymbol() { }

	public bool Equals(TypeSymbol? other)
	{
		if (other is null) return false;

		if (other is PredefinedTypeSymbol predefined) other = predefined;

		return ReferenceEquals(this is PredefinedTypeSymbol type ? type.UnderlyingType : this, other);
	}

	public override bool Equals(object? obj)
	{
		return Equals(obj as TypeSymbol);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Members, ContainingSymbol);
	}

	public static bool operator ==(TypeSymbol? left, TypeSymbol? right)
	{
		return !ReferenceEquals(left, null) && left.Equals(right);
	}

	public static bool operator !=(TypeSymbol? left, TypeSymbol? right)
	{
		return !(left == right);
	}
}