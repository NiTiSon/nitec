namespace NiteCompiler.CodeAnalysis.Symbols;

public sealed class ModuleSymbol : Symbol
{
	public string FullName { get; }

	public bool IsGlobalModule => FullName == string.Empty;

	public ModuleSymbol(string fullName)
	{
		FullName = fullName;
	}

	public override Symbol? ContainingSymbol => null;
	public override SymbolKind Kind => SymbolKind.Module;
}