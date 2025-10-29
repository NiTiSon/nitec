using System.Linq;
using NiteCompiler.CodeAnalysis.Symbols.Source;

namespace NiteCompiler.CodeAnalysis.Symbols;

public sealed class ModuleManager
{
	private readonly GlobalScope _globalScope;

	private SourceLibrarySymbol Internal => _globalScope.ThisLibrary;

	internal ModuleManager(GlobalScope scope)
	{
		_globalScope = scope;
		GetModuleDeclaration(string.Empty);
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