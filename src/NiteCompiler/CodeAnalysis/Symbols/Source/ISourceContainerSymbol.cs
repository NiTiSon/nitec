using System.Collections.Generic;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal interface ISourceContainerSymbol : IContainerSymbol, ISourceSymbol
{
	new ICollection<IMemberSymbol> Members { get; }
}