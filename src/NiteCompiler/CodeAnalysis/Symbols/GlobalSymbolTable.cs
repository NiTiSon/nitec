using System;
using System.Collections.Generic;
using System.Linq;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Symbols;

public sealed class GlobalSymbolTable
{
	private readonly Dictionary<string, LibrarySymbol> _libraries;

	public LibrarySymbol Library { get; }

	public IEnumerable<LibrarySymbol> Libraries => _libraries.Values;

	/// <summary>
	/// Return modules across all libraries in compilation.
	/// </summary>
	/// <remarks>
	/// The reference equality of modules is not guarantied, but other symbols must be fine.
	/// </remarks>
	public IEnumerable<ModuleSymbol> Modules =>
		_libraries
			.SelectMany(t => t.Value.Modules)
			.GroupBy(t => t.FullName)
			.Select(ModuleSymbol.Combine);

	public GlobalSymbolTable(string ownLibraryName)
	{
		_libraries = [];
		_libraries.Add(ownLibraryName, Library = new LibrarySymbol(ownLibraryName));
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
			ModuleSymbol? module = libs.Modules.FirstOrDefault(t => t.FullName == name);
			if (module != null) yield return module;
		}
	}

	/// <summary>
	/// Returns or add new module symbol in current library.
	/// </summary>
	/// <param name="name">Full module name.</param>
	public ModuleSymbol GetOrAddInternalModule(string name)
	{
		return Library.GetOrAddModule(name);
	}
}