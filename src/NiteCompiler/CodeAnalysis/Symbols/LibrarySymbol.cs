using System.Collections.Generic;
using System.Linq;

namespace NiteCompiler.CodeAnalysis.Symbols;

public sealed class LibrarySymbol : Symbol
{
	private readonly List<ModuleSymbol> _modules;

	public string Name { get; }
	public IEnumerable<ModuleSymbol> Modules => _modules;

	public LibrarySymbol(string name)
	{
		Name = name;
		_modules = [];
	}

	public override Symbol? ContainingSymbol => null;
	public override SymbolKind Kind => SymbolKind.Library;

	internal ModuleSymbol GetOrAddModule(string? name)
	{
		if (string.IsNullOrEmpty(name))
		{
			name = "<global>";
		}

		var module = Modules.FirstOrDefault(t => t.FullName == name);

		return module ?? AddModule(name);
	}

	private ModuleSymbol AddModule(string name)
	{
		ModuleSymbol module = new(name);
		_modules.Add(module);
		return module;
	}
}