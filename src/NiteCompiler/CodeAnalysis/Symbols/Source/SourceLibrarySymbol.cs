using NiteCompiler.CodeAnalysis.Declarations;
using NiteCompiler.Compilation;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceLibrarySymbol : LibrarySymbol
{
	private readonly NiteCompilation _compilation;
	public override string Name { get; }
	public override Symbol? ContainingSymbol => null;

	public override SymbolKind Kind => SymbolKind.Library;
	public SourceModuleSymbol GlobalModule { get; }

	public SourceLibrarySymbol(NiteCompilation compilation, MergedModuleDeclaration rootModule, string name)
	{
		Name = name;
		_compilation = compilation;

		GlobalModule = new(this, rootModule, MetadataFacts.GlobalModuleInternalName);
	}
}