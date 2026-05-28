using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace NiteCompiler.CodeAnalysis.Symbols;

public sealed class SizedArrayTypeSymbol : BaseArrayTypeSymbol
{
	public override TypeKind TypeKind => TypeKind.SizedArray;
	public override SymbolKind Kind => SymbolKind.Array;
	public override TypeSymbol ElementsType { get; }

	[NotNull]
	public override ulong? Length { get; }

	public override bool IsUnsized => false;

	public override string Name => $"[{ElementsType.Name}; {Length}]";

	internal SizedArrayTypeSymbol(TypeSymbol elementsType, ulong length)
	{
		ElementsType = elementsType;
		Length = length;
	}
}
