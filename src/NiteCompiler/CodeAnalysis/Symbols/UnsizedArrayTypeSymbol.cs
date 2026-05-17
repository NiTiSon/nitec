namespace NiteCompiler.CodeAnalysis.Symbols;

public sealed class UnsizedArrayTypeSymbol : BaseArrayTypeSymbol
{
	public override TypeKind TypeKind => TypeKind.UnsizedArray;
	public override SymbolKind Kind => SymbolKind.Array;
	public override TypeSymbol ElementsType { get; }
	public override int? Length => null;

	public override bool IsUnsized => true;

	internal UnsizedArrayTypeSymbol(TypeSymbol elementsType)
	{
		ElementsType = elementsType;
	}
}