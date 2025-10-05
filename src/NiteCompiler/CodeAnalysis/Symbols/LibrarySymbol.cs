using System.Collections.Generic;
using System.Linq;

namespace NiteCompiler.CodeAnalysis.Symbols;

public sealed class LibrarySymbol : Symbol
{
	private readonly List<ModuleSymbol> _modules;

	public override string Name { get; }
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
			name = WellKnownSemantic.GlobalModuleName;
		}

		var module = Modules.FirstOrDefault(t => t.Name == name);

		return module ?? AddModule(name);
	}

	private ModuleSymbol AddModule(string name)
	{
		ModuleSymbol module = new(name)
		{
			ContainingLibrary = this
		};
		_modules.Add(module);
		return module;
	}
}