namespace Nlr.Compiler.Symbols;

public sealed class ModuleSymbol : Symbol
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