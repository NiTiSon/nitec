namespace Nlr.Compiler.NiteCode.Symbols;

public interface IFunctionSymbol : ISymbol
{
	int Arity { get; }
	
	bool IsGeneric { get; }
	
	bool ReturnsVoid { get; }
	
	ITypeSymbol ReturnType { get; }
	
	// IFunctionSymbol Construct(params IGenericSymbol[] arguments);
	
	DynamicLinkData? DynamicLinkData { get; }
}