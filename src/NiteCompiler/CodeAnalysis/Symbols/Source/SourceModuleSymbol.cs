namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceModuleSymbol : ContainerSymbol
{
	public override SymbolKind Kind => SymbolKind.Module;
}