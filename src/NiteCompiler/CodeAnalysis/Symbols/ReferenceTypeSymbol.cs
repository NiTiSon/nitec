namespace NiteCompiler.CodeAnalysis.Symbols;

public sealed class ReferenceTypeSymbol : BaseReferenceTypeSymbol
{
	public override TypeKind TypeKind => TypeKind.Reference;
	public override SymbolKind Kind => SymbolKind.Reference;
	public override TypeSymbol PointsTo { get; }
	public override bool IsMutable { get; }
	public override bool IsNullable { get; }
	public override bool IsThickPointer => PointsTo.IsUnsized /*|| PointsTo.IsAbstract*/;
	public LifetimeSymbol? Lifetime { get; }

	internal ReferenceTypeSymbol(TypeSymbol pointsTo, bool isMutable, bool isNullable, LifetimeSymbol? lifetime = null)
	{
		PointsTo = pointsTo;
		IsMutable = isMutable;
		IsNullable = isNullable;
		Lifetime = lifetime;
	}

	public override string ToDisplayString(SymbolFormat format = SymbolFormat.Default)
	{
		int displayIndex = (IsNullable ? 1 : 0) + (!IsMutable ? 2 : 0);
		string lifetime = Lifetime != null ? $"{Lifetime.ToDisplayString(format)} " : "";

		return $"{Display[displayIndex]}{lifetime}{PointsTo.ToDisplayString(format)}";
	}

	private static string[] Display =
	[
		"&",
		"&?",
		"&const ",
		"&const? "
	];
}