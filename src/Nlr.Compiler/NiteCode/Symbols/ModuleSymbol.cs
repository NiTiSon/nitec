namespace Nlr.Compiler.NiteCode.Symbols;

public sealed class ModuleSymbol : ISymbol
{
	public override string Name { get; }
	public override ModuleSymbol ModuleContainer { get; }

	public ModuleSymbol(string name, ModuleSymbol? moduleContainer)
	{
		Name = name;
		ModuleContainer = moduleContainer ?? Global;
	}

	private ModuleSymbol()
	{
		Name = null!;
		ModuleContainer = null!;
	}

	public static readonly ModuleSymbol Global = new();

	public override SymbolKind Kind => SymbolKind.Module;
}