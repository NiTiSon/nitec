using System;
using System.Collections.Generic;
using System.Linq;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Symbols;

public sealed class GlobalSymbolTable
{
	private readonly Dictionary<string, LibrarySymbol> _libraries;

	public GlobalSymbolTable(string ownLibraryName)
	{
		_libraries = [];
		_libraries.Add(ownLibraryName, new LibrarySymbol(ownLibraryName));
	}

	public void AddLibrary(LibrarySymbol library)
	{
		_libraries[library.Name] = library;
	}

	/// <summary>
	/// Returns module symbols from all libraries (including current) with the same module name.
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	public IEnumerable<ModuleSymbol> GetModules(string name)
	{
		foreach (LibrarySymbol libs in _libraries.Values)
		{
			ModuleSymbol? module = libs.Modules.FirstOrDefault(t => t.FullName == name);
			if (module != null) yield return module;
		}
	}
}