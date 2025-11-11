using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NiteCompiler.CodeAnalysis.Symbols.Source;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Symbols;

public sealed class ModuleManager
{
	private readonly GlobalScope _globalScope;
	private readonly Dictionary<string, MergedModuleSymbol> _cachedMergedModules = [];

	private SourceLibrarySymbol Internal => _globalScope.ThisLibrary;

	internal ModuleManager(GlobalScope scope)
	{
		_globalScope = scope;
	}

	/// <summary>
	/// Returns set of modules from all libraries, including this.
	/// </summary>
	/// <param name="name">Module name to find.</param>
	/// <param name="exists">Sets to true when at least one found.</param>
	/// <returns>Set of modules with same names.</returns>
	/// <remarks>
	/// Do not call before Declaration pass.
	/// </remarks>
	private ImmutableArray<ModuleSymbol> GetModuleDeclarationWithinAllLibraries(string name, out bool exists)
	{
		var builder = ImmutableArray.CreateBuilder<ModuleSymbol>();

		SourceModuleSymbol? internalModule = Internal.Modules.FirstOrDefault(t => t.Name == name);

		if (internalModule != null) builder.Add(internalModule);

		foreach (ModuleSymbol module in _globalScope.Dependencies.SelectMany(t => t.Modules))
		{
			if (module.Name == name)
			{
				builder.Add(module);
			}
		}

		ImmutableArray<ModuleSymbol> modules = builder.ToImmutable();
		exists = modules.Length != 0;
		return modules;
	}

	public MergedModuleSymbol? GetModule(string name)
	{
		if (_cachedMergedModules.TryGetValue(name, out var module))
		{
			return module;
		}
		else
		{
			ImmutableArray<ModuleSymbol> moduleDeclarations
				= GetModuleDeclarationWithinAllLibraries(name, out bool exists);

			if (!exists) return null;

			MergedModuleSymbol merged = new(name, moduleDeclarations);

			_cachedMergedModules.Add(name, merged);
			return merged;

		}
	}

	internal SourceModuleSymbol GetSourceModuleSymbol(ModuleDeclarationSyntax module)
		=> GetSourceModuleSymbol(module.Name.GetName());

	internal SourceModuleSymbol GetSourceModuleSymbol(string name)
	{
		if (TryGetSourceModuleSymbol(name, out SourceModuleSymbol? sourceModule))
		{
			return sourceModule;
		}
		else
		{
			sourceModule = new(Internal, name);
			Internal.Modules.Add(sourceModule);
			return sourceModule;
		}
	}

	internal bool TryGetSourceModuleSymbol(string name, [NotNullWhen(true)] out SourceModuleSymbol? symbol)
	{
		symbol = Internal.Modules.FirstOrDefault(t => t.Name == name);

		return symbol != null;
	}
}