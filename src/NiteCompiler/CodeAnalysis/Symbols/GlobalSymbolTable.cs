using System;
using System.Collections.Generic;
using System.Linq;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Symbols;

public sealed class GlobalSymbolTable
{
	private readonly Dictionary<string, LibrarySymbol> _libraries;

	public LibrarySymbol Library { get; }

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