using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Symbols;

internal sealed class GlobalSymbolTable
{
	private readonly Dictionary<string, LibrarySymbol> _libraries = [];
	private readonly Dictionary<DefaultType, PredefinedTypeSymbol> _defaultTypes = [];

	public LibrarySymbol Library { get; }

	public IEnumerable<LibrarySymbol> Libraries => _libraries.Values;

	public DiagnosticBag Diagnostics { get; } = [];

	/// <summary>
	/// Return modules across all libraries in compilation.
	/// </summary>
	/// <remarks>
	/// The reference equality of modules is not guarantied, but other symbols must be fine.
	/// </remarks>
	public IEnumerable<ModuleSymbol> Modules =>
		_libraries
			.SelectMany(t => t.Value.Modules)
			.GroupBy(t => t.Name)
			.Select(ModuleSymbol.Combine);

	public GlobalSymbolTable(string ownLibraryName)
	{
		_libraries.Add(ownLibraryName, Library = new LibrarySymbol(ownLibraryName));

		LibrarySymbol stdlib = new("stdlib");
		_libraries.Add(stdlib.Name, stdlib);

		ModuleSymbol numericsModule = stdlib.GetOrAddModule(WellKnownSemantic.NumericsModuleName);
		_defaultTypes[DefaultType.I32] = new(numericsModule, WellKnownSemantic.I32TypeName, DefaultType.I32);
		foreach (PredefinedTypeSymbol defType in _defaultTypes.Values)
		{
			numericsModule.AddMember(defType);
		}
	}

	public void AddLibrary(LibrarySymbol library)
	{
		_libraries[library.Name] = library;
	}

	/// <summary>
	/// Returns module symbols from all libraries (including current) with the same module name.
	/// </summary>
	/// <param name="name">Full module name.</param>
	public IEnumerable<ModuleSymbol> GetModules(string name)
	{
		foreach (LibrarySymbol libs in _libraries.Values)
		{
			ModuleSymbol? module = libs.Modules.FirstOrDefault(t => t.Name == name);
			if (module != null) yield return module;
		}
	}

	public ModuleSymbol GetCombinedModule(string name)
	{
		return ModuleSymbol.Combine(GetModules(name));
	}

	/// <summary>
	/// Returns or add new module symbol in current library.
	/// </summary>
	/// <param name="name">Full module name.</param>
	public ModuleSymbol GetOrAddInternalModule(string name)
	{
		return Library.GetOrAddModule(name);
	}

	public PredefinedTypeSymbol GetPredefinedType(DefaultType defaultType)
	{
		_defaultTypes.TryGetValue(defaultType, out var result);
		return result ?? throw new NotImplementedException();
	}
}