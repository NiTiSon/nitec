using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using NiteLang.Metadata;

namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class TypeSymbol : Symbol
{
	public sealed override SymbolKind Kind => SymbolKind.Type;
	public abstract ImmutableArray<Symbol> Members { get; }
	public abstract TypeSymbol? Parent { get; }
	public abstract SpecialType SpecialType { get; }

	private protected TypeSymbol() { }
}