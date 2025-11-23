using System;
using System.Collections.Immutable;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceLibrarySymbol : LibrarySymbol
{
	private readonly FreezableArray<ModuleSymbol> _modules = new();

	public override string Name { get; }

	public override ImmutableArray<ModuleSymbol> Modules => _modules;
	public override bool IsExplicitlyDeclaredAsCoreLibrary { get; }

	public SourceLibrarySymbol(string name, bool isCoreLibrary)
	{
		Name = name;
		IsExplicitlyDeclaredAsCoreLibrary = isCoreLibrary;
	}

	internal void AddModule(SourceModuleSymbol module)
	{
		_modules.Add(module);
	}
}