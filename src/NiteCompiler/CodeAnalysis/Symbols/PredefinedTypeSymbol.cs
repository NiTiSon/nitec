using System.Collections.Generic;

namespace NiteCompiler.CodeAnalysis.Symbols;

public sealed class PredefinedTypeSymbol : TypeSymbol
{
	public TypeSymbol UnderlyingType { get; internal set; }
	public PredefinedType Type { get; }
	public override IEnumerable<IMemberSymbol> Members => UnderlyingType.Members;
	public override IContainerSymbol? ContainingSymbol => UnderlyingType.ContainingSymbol;
	public override TypeSymbol? Parent => UnderlyingType.Parent;

	internal PredefinedTypeSymbol(TypeSymbol underlyingType, PredefinedType type)
	{
		UnderlyingType = underlyingType;
		Type = type;
	}
}