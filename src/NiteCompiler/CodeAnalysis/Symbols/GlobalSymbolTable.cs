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
	private readonly Dictionary<PredefinedType, TypeSymbol> _defaultTypes = [];

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
	}

	public void ResolvePredefinedTypes()
	{
		foreach (PredefinedType type in new[] { PredefinedType.I32, PredefinedType.F32 })
		{
			(string moduleName, string typeName) = SyntaxFacts.GetModuleAndTypeName(type);
			ModuleSymbol module = GetCombinedModule(moduleName);

			TypeSymbol? retusa = module.Types.FirstOrDefault(t => t.Name == typeName);
			if (retusa == null)
			{
				Diagnostics.ReportUnresolvedPredefinedSymbol($"{moduleName}::{typeName}");
			}
			_defaultTypes[type] = retusa;
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

	/// <summary>
	/// Try to find corresponding type for predefined type.
	/// </summary>
	/// <param name="type">Predefined type to find.</param>
	/// <returns>Named type symbol from this or dependency library.</returns>
	public TypeSymbol? GetPredefinedType(PredefinedType type)
	{
		if (_defaultTypes.TryGetValue(type, out TypeSymbol? predefinedType))
		{
			return predefinedType;
		}

		return TryResolvePredefinedType();

		TypeSymbol? TryResolvePredefinedType()
		{
			string modulePath;
			string typeName;
			switch (type)
			{
				case PredefinedType.I32:
					modulePath = WellKnownSemantic.NumericsModuleName;
					typeName = WellKnownSemantic.I32TypeName;
					break;
				default:
					throw new InvalidEnumArgumentException();
			}

			ModuleSymbol module = GetCombinedModule(modulePath);

			TypeSymbol? symbol = module.Types.FirstOrDefault(t => t.Name == typeName);

			if (symbol != null)
			{
				_defaultTypes[type] = symbol;
			}

			return symbol;
		}
	}
}