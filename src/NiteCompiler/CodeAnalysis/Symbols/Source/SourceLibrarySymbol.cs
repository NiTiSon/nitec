using NiteCompiler.Compilation;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceLibrarySymbol : ContainerSymbol
{
	private readonly NiteCompilation _compilation;
	public string Name { get; }

	public override SymbolKind Kind => SymbolKind.Library;

	public SourceLibrarySymbol(NiteCompilation compilation, string name)
	{
		Name = name;
		_compilation = compilation;
	}
}