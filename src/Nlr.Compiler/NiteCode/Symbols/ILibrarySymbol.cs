using System.Collections.Generic;
using System.Collections.Immutable;

namespace Nlr.Compiler.NiteCode.Symbols;

public interface ILibrarySymbol : ISymbol
{
	LibraryIdentity Identity { get; }
	
	IEnumerable<IModuleSymbol> Modules { get; }
	
	ImmutableArray<LibraryIdentity> ReferencedLibraries { get; }
	
	
}