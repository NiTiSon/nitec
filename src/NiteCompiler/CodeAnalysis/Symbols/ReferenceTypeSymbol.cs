namespace NiteCompiler.CodeAnalysis.Symbols;

public sealed class ReferenceTypeSymbol : BaseReferenceTypeSymbol
{
	public override TypeKind TypeKind => TypeKind.Reference;
	public override SymbolKind Kind => SymbolKind.Reference;
	public override TypeSymbol PointsTo { get; }
	public override bool IsMutable { get; }
	public override bool IsNullable { get; }
	public override bool IsThickPointer => PointsTo.IsUnsized /*|| PointsTo.IsAbstract*/;

	internal ReferenceTypeSymbol(TypeSymbol pointsTo, bool isMutable, bool isNullable)
	{
		PointsTo = pointsTo;
		IsMutable = isMutable;
		IsNullable = isNullable;
	}

	public override string ToDisplayString(SymbolFormat format = SymbolFormat.Default)
	{
		int displayIndex = (IsNullable ? 1 : 0) + (!IsMutable ? 2 : 0);

		return Display[displayIndex] + PointsTo.ToDisplayString(format);
	}

	private static string[] Display =
	[
		"&",
		"&?",
		"&const ",
		"&const? "
	];
}