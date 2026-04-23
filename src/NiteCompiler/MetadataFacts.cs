namespace NiteCompiler;

public static class MetadataFacts
{
	public const string GlobalModuleInternalName = "<global>";
	public const string ConstructorInternalNamePrefix = "<k>";
	public const string OperatorInternalNamePrefix = "<op>";

	public const string BooleanTypeSimpleName = "bool";
	public const string SignedIntegralType8BitSimpleName = "i8";
	public const string SignedIntegralType16BitSimpleName = "i16";
	public const string SignedIntegralType32BitSimpleName = "i32";
	public const string SignedIntegralType64BitSimpleName = "i64";
	public const string SignedIntegralTypeNativeSizeSimpleName = "isize";
	public const string UnsignedIntegralType8BitSimpleName = "u8";
	public const string UnsignedIntegralType16BitSimpleName = "u16";
	public const string UnsignedIntegralType32BitSimpleName = "u32";
	public const string UnsignedIntegralType64BitSimpleName = "u64";
	public const string UnsignedIntegralTypeNativeSizeSimpleName = "usize";
	public const string FloatPointType8BitSimpleName = "f8";
	public const string FloatPointType16BitSimpleName = "f16";
	public const string FloatPointType32BitSimpleName = "f32";
	public const string FloatPointType64BitSimpleName = "f64";

	internal static string? GetMetadataName(SpecialType specialType)
	{
		return specialType switch
		{
			SpecialType.StdBoolean => BooleanTypeSimpleName,
			SpecialType.StdNumericsSInt8 => SignedIntegralType8BitSimpleName,
			SpecialType.StdNumericsSInt16 => SignedIntegralType16BitSimpleName,
			SpecialType.StdNumericsSInt32 => SignedIntegralType32BitSimpleName,
			SpecialType.StdNumericsSInt64 => SignedIntegralType64BitSimpleName,
			SpecialType.StdNumericsSNativeInt => SignedIntegralTypeNativeSizeSimpleName,
			SpecialType.StdNumericsUInt8 => UnsignedIntegralType8BitSimpleName,
			SpecialType.StdNumericsUInt16 => UnsignedIntegralType16BitSimpleName,
			SpecialType.StdNumericsUInt32 => UnsignedIntegralType32BitSimpleName,
			SpecialType.StdNumericsUInt64 => UnsignedIntegralType64BitSimpleName,
			SpecialType.StdNumericsUNativeInt => UnsignedIntegralTypeNativeSizeSimpleName,
			SpecialType.StdNumericsFloat16 => FloatPointType16BitSimpleName,
			SpecialType.StdNumericsFloat32 => FloatPointType32BitSimpleName,
			SpecialType.StdNumericsFloat64 => FloatPointType64BitSimpleName,
			_ => null
		};
	}
}