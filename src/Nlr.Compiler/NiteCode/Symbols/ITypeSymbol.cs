using System.Collections.Immutable;

namespace Nlr.Compiler.NiteCode.Symbols;

public interface ITypeSymbol : IContainerSymbol
{
	ITypeSymbol? BaseType { get; }
	
	ImmutableArray<ITypeSymbol> Interfaces { get; }
	
	ISymbol? GetInterfaceImplementation(ISymbol interfaceMember);
}