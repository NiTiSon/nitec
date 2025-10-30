using NiteCompiler.CodeAnalysis.Symbols.Source;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class ModuleBinder : Binder
{
	public SourceModuleSymbol Module { get; }

	public ModuleBinder(Binder? parent, Compilation compilation, SourceModuleSymbol module) : base(parent, compilation)
	{
		Module = module;
	}
}