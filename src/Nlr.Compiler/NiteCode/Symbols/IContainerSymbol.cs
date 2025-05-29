using System.Collections.Immutable;

namespace Nlr.Compiler.NiteCode.Symbols;

public interface IContainerSymbol : ISymbol
{
	ImmutableArray<ISymbol> GetMembers();
	
	ImmutableArray<ISymbol> GetMembers(string name);
	
	public bool IsType { get; }
	
	public bool IsModule { get; }
}