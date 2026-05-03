namespace NiteCompiler.CodeAnalysis.Symbols;

public sealed class ReferenceTypeSymbol : BaseReferenceTypeSymbol
{
	public override TypeSymbol PointsTo { get; }
	public override bool IsNullable { get; }
	public override bool IsMutable { get; }
	// public override bool IsFatPointer => PointsTo.IsUnsized || Points.To.IsAbstract;

	internal ReferenceTypeSymbol(TypeSymbol pointsTo, bool isNullable, bool isMutable)
	{
		PointsTo = pointsTo;
		IsNullable = isNullable;
		IsMutable = isMutable;
	}
}