using System;
using System.Collections.Immutable;
using System.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class TypeSymbol : ContainerSymbol, IEquatable<TypeSymbol>
{
	public abstract TypeKind TypeKind { get; }

	public virtual SpecialType SpecialType => SpecialType.None;

	public bool IsVoidType => SpecialType == SpecialType.StdVoid;
	public virtual bool IsUnsized => false;

	public sealed override ModuleSymbol? GetNestedModule(string name)
	{
		return null;
	}

	public sealed override ImmutableArray<ModuleSymbol> GetNestedModules()
	{
		return [];
	}

	public bool Equals(TypeSymbol? other)
	{
		return Equals(lhs: this, other, TypeComparison.None);
	}

	public bool Equals(TypeSymbol? other, TypeComparison comparison)
	{
		return Equals(lhs: this, other, comparison);
	}

	public static bool Equals(TypeSymbol? lhs, TypeSymbol? rhs, TypeComparison comparison)
	{
		if (ReferenceEquals(lhs, rhs))
		{
			return true;
		}

		if (lhs is null || rhs is null)
		{
			return false;
		}

		return false;
	}

	[Obsolete("Use Equals method instead.")]
	public static bool operator ==(TypeSymbol? lhs, TypeSymbol? rhs)
	{
		Debug.Fail("Should never get here.");
		throw new UnreachableException();
	}

	[Obsolete("Use Equals method instead.")]
	public static bool operator !=(TypeSymbol? lhs, TypeSymbol? rhs)
	{
		Debug.Fail("Should never get here.");
		throw new UnreachableException();
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

		// TODO: REWORK THIS SHI!!!!!
		if (!format.HasFlag(SymbolFormat.OmitContainer) && ContainingSymbol != null)
		{
			if (ContainingSymbol is ModuleSymbol { IsGlobalModule: true })
			{
				if (format.HasFlag(SymbolFormat.EmitGlobalModule))
				{
					result = $"{ContainingSymbol.ToDisplayString(format)}::{result}";
				}
				else if (format.HasFlag(SymbolFormat.IncludeLibrary) && ContainingLibrary != null)
				{
					result = ContainingLibrary.ToDisplayString(format) + result;
				}
			}
			else
			{
				result = $"{ContainingSymbol.ToDisplayString(format)}::{result}";
			}
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