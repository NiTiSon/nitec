namespace NiteCompiler.CodeAnalysis.Symbols;

public sealed class UnsizedArrayTypeSymbol : BaseArrayTypeSymbol
{
	public override TypeSymbol ElementsType { get; }
	public override int? Length => null;

	public override bool IsUnsized => true;

	internal UnsizedArrayTypeSymbol(TypeSymbol elementsType)
	{
		ElementsType = elementsType;
	}
}