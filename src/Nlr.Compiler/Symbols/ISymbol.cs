namespace Nlr.Compiler.Symbols;

public interface ISymbol
{
	/// <summary>
	/// Symbol name. Can be empty.
	/// </summary>
	string Name { get; }
	
	SymbolKind Kind { get; }
	
	ISymbol? ContainingSymbol { get; }
	
	IModuleSymbol? ContainingModule { get; }
	
	bool IsDefinition { get; }
	
	bool IsStatic { get; }
	
	bool IsVirtual { get; }
	
	bool IsAbstract { get; }
	
	bool IsOverride { get; }
	
	bool IsSealed { get; }
	
	bool Isnogeneric { get; }
	
	bool CanBeReferencedByName { get; }
	
	
}