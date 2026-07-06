using System.Collections.Immutable;
using System.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class TypeSymbol : ContainerSymbol
{
	public override SymbolKind Kind => SymbolKind.Type;

	public virtual SpecialType SpecialType => SpecialType.None;

	public virtual bool IsUnsized => false;

	public override bool Equals(object? obj)
	{
		if (obj is TypeSymbol other && SpecialType != SpecialType.None && other.SpecialType != SpecialType.None)
		{
			return SpecialType == other.SpecialType;
		}
		return ReferenceEquals(this, obj);
	}

	public override int GetHashCode()
	{
		if (SpecialType != SpecialType.None)
		{
			return (int)SpecialType;
		}
		return base.GetHashCode();
	}

	public static bool operator ==(TypeSymbol? left, TypeSymbol? right)
	{
		if (ReferenceEquals(left, right)) return true;
		if (left is null || right is null) return false;
		return left.Equals(right);
	}

	public static bool operator !=(TypeSymbol? left, TypeSymbol? right)
	{
		return !(left == right);
	}

	public override ModuleSymbol? GetNestedModule(string name)
	{
		return null;
	}

	public override ImmutableArray<ModuleSymbol> GetNestedModules()
	{
		return [];
	}

	public override string ToDisplayString(SymbolFormat format = SymbolFormat.Default)
	{
		Debug.Assert(format.IsValid);

		string? result = null;
		if (format.HasFlag(SymbolFormat.PreferShortSpecialTypeName) && SpecialType != SpecialType.None)
		{
			result = MetadataFacts.GetMetadataName(SpecialType);
		}

		if (result == null)
		{
			result = Name;
		}
		else
		{
			return result;
		}

		if (!format.HasFlag(SymbolFormat.OmitContainer) && ContainingSymbol != null)
		{
			result = $"{ContainingSymbol.ToDisplayString(format)}::{result}";
		}
		else if (format.HasFlag(SymbolFormat.IncludeLibrary) && ContainingLibrary != null)
		{
			result = ContainingLibrary.ToDisplayString(format) + result;
		}

		return result;
	}

	public override void Accept(SymbolVisitor visitor)
	{
		visitor.VisitType(this);
	}

	public override TResult? Accept<TResult>(SymbolVisitor<TResult> visitor) where TResult : default
	{
		return visitor.VisitType(this);
	}

	public override TResult? Accept<TResult, TArgument>(SymbolVisitor<TResult, TArgument> visitor, TArgument arg) where TResult : default
	{
		return visitor.VisitType(this, arg);
	}
}