using System.Collections.Generic;
using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.CodeAnalysis.Text;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class Symbol
{
	public abstract SymbolKind Kind { get; }

	/// <summary>
	/// Locations where symbols is defined.
	/// </summary>
	public virtual ImmutableArray<Location> Locations => [];

	/// <summary>
	/// User-visible name or empty.
	/// </summary>
	public virtual string Name => string.Empty;

	/// <summary>
	/// Name as it appears in metadata.
	/// </summary>
	public virtual string MetadataName => Name;

	public abstract Symbol? ContainingSymbol { get; }

	private protected Symbol() {}
}