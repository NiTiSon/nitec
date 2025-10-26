using System.Collections.Generic;

namespace NiteCompiler.CodeAnalysis.Symbols;

/// <summary>
/// Symbol implemented this interface can contain members.
/// </summary>
public interface IContainerSymbol
{
	IEnumerable<IMemberSymbol> Members { get; }
}