namespace NiteCompiler.CodeAnalysis.Symbols;

/// <summary>
/// Symbol implemented this interface is a member.
/// </summary>
public interface IMemberSymbol
{
	IContainerSymbol? ContainingSymbol { get; }
}