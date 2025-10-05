namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class TypeSymbol : Symbol
{
	public override SymbolKind Kind => SymbolKind.Type;
	public abstract override Symbol ContainingSymbol { get; }
	public ModuleSymbol ContainingModule
	{
		get
		{
			Symbol s = ContainingSymbol;
			while (s is not ModuleSymbol)
			{
				s = s.ContainingSymbol;
			}

			return (ModuleSymbol)s;
		}
	}

	public override string ToSignatureString()
	{
		return Name;
	}
}