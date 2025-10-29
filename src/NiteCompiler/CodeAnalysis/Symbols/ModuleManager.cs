using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NiteCompiler.CodeAnalysis.Symbols.Source;

namespace NiteCompiler.CodeAnalysis.Symbols;

public sealed class ModuleManager
{
	private readonly GlobalScope _globalScope;
	private readonly Dictionary<string, ImmutableArray<ModuleSymbol>> _cachedModuleInclude = [];

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
	public ImmutableArray<ModuleSymbol> GetModuleDeclarationWithinAllLibraries(string name, out bool exists)
	{
		if (_cachedModuleInclude.TryGetValue(name, out ImmutableArray<ModuleSymbol> result))
		{
			exists = result.Length != 0;
			return result;
		}
		else
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

			_cachedModuleInclude[name] = result = builder.ToImmutable();
			exists = result.Length != 0;
			return result;
		}
	}

	internal SourceModuleSymbol GetModuleDeclaration(string moduleName)
	{
		SourceModuleSymbol? sourceModuleSymbol = Internal.Modules.FirstOrDefault(t => t.Name == moduleName);

		if (sourceModuleSymbol is null)
		{
			sourceModuleSymbol = new(Internal, moduleName);
			Internal.Modules.Add(sourceModuleSymbol);
		}

		return sourceModuleSymbol;
	}
}