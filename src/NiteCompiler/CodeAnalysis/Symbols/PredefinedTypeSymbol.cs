using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;

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

	[Pure]
	public static (string moduleName, string typeName) GetNameInfo(PredefinedType type)
	{
		return type switch
		{
			PredefinedType.NeverReturn => (WellKnownMembers.StandardModuleName, WellKnownMembers.NeverReturnTypeName),
			PredefinedType.Void => (WellKnownMembers.StandardModuleName, WellKnownMembers.VoidTypeName),
			PredefinedType.I8 => (WellKnownMembers.NumericsModuleName, WellKnownMembers.I8TypeName),
			PredefinedType.I16 => (WellKnownMembers.NumericsModuleName, WellKnownMembers.I16TypeName),
			PredefinedType.I32 => (WellKnownMembers.NumericsModuleName, WellKnownMembers.I32TypeName),
			PredefinedType.I64 => (WellKnownMembers.NumericsModuleName, WellKnownMembers.I64TypeName),
			PredefinedType.U8 => (WellKnownMembers.NumericsModuleName, WellKnownMembers.U8TypeName),
			PredefinedType.U16 => (WellKnownMembers.NumericsModuleName, WellKnownMembers.U16TypeName),
			PredefinedType.U32 => (WellKnownMembers.NumericsModuleName, WellKnownMembers.U32TypeName),
			PredefinedType.U64 => (WellKnownMembers.NumericsModuleName, WellKnownMembers.U64TypeName),
			PredefinedType.F16 => (WellKnownMembers.NumericsModuleName, WellKnownMembers.F16TypeName),
			PredefinedType.F32 => (WellKnownMembers.NumericsModuleName, WellKnownMembers.F32TypeName),
			PredefinedType.F64 => (WellKnownMembers.NumericsModuleName, WellKnownMembers.F64TypeName),
			_ => throw new InvalidEnumArgumentException()
		};
	}
}