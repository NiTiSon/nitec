using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class LibrarySymbol : Symbol
{
	public abstract override string Name { get; }
	public sealed override SymbolKind Kind => SymbolKind.Library;
	public abstract ImmutableArray<ModuleSymbol> Modules { get; }
	public sealed override Symbol? ContainingSymbol => null;

	/// <summary>
	/// Explicit core library can be used to import standard types.
	/// </summary>
	/// <remarks>
	/// Only current library can be explicit core library.
	/// </remarks>
	public virtual bool IsExplicitlyDeclaredAsCoreLibrary => false;

	private protected LibrarySymbol() {}
}