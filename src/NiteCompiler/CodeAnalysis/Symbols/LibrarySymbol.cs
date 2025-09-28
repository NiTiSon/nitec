using System.Collections.Generic;

namespace NiteCompiler.CodeAnalysis.Symbols;

public sealed class LibrarySymbol : Symbol
{
	public string Name { get; }
	public IEnumerable<ModuleSymbol> Modules { get; }

	public LibrarySymbol(string name)
	{
		Name = name;
		Modules = [];
	}

	public override Symbol? ContainingSymbol => null;
	public override SymbolKind Kind => SymbolKind.Library;
}