using System.Collections.Generic;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal interface ISourceContainerSymbol : IContainerSymbol
{
	new ICollection<IMemberSymbol> Members { get; }
}