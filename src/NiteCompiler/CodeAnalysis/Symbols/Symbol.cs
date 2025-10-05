namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class Symbol
{
	/// <summary>
	/// Gets symbol that contains this symbol as member.
	/// </summary>
	public abstract Symbol? ContainingSymbol { get; }

	public abstract string Name { get; }

	/// <summary>
	/// Gets symbol type.
	/// </summary>
	public abstract SymbolKind Kind { get; }

	public override string ToString()
	{
		return $"{Kind} {ToSignatureString()}";
	}

	public virtual string ToSignatureString()
	{
		return Name;
	}
}