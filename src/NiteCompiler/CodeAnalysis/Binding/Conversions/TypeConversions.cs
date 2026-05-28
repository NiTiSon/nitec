using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.CodeAnalysis.Binding.Conversions;

internal static class TypeConversions
{
	public static ConversionKind ClassifyConversion(TypeSymbol fromType, TypeSymbol toType)
	{
		if (fromType.Equals(toType))
		{
			return ConversionKind.Identity;
		}

		if (fromType.SpecialType == SpecialType.None || toType.SpecialType == SpecialType.None)
		{
			return ConversionKind.NoConversion;
		}

		SpecialType from = fromType.SpecialType;
		SpecialType to = toType.SpecialType;

		if (from == to)
		{
			return ConversionKind.Identity;
		}

		return (from, to) switch
		{
			// Implicit: signed integer widening
			(SpecialType.StdNumericsSInt8, SpecialType.StdNumericsSInt16) => ConversionKind.ImplicitNumeric,
			(SpecialType.StdNumericsSInt8, SpecialType.StdNumericsSInt32) => ConversionKind.ImplicitNumeric,
			(SpecialType.StdNumericsSInt8, SpecialType.StdNumericsSInt64) => ConversionKind.ImplicitNumeric,
			(SpecialType.StdNumericsSInt8, SpecialType.StdNumericsSNativeInt) => ConversionKind.ImplicitNumeric,
			(SpecialType.StdNumericsSInt16, SpecialType.StdNumericsSInt32) => ConversionKind.ImplicitNumeric,
			(SpecialType.StdNumericsSInt16, SpecialType.StdNumericsSInt64) => ConversionKind.ImplicitNumeric,
			(SpecialType.StdNumericsSInt16, SpecialType.StdNumericsSNativeInt) => ConversionKind.ImplicitNumeric,
			(SpecialType.StdNumericsSInt32, SpecialType.StdNumericsSInt64) => ConversionKind.ImplicitNumeric,
			(SpecialType.StdNumericsSInt32, SpecialType.StdNumericsSNativeInt) => ConversionKind.ImplicitNumeric,

			// Implicit: unsigned integer widening
			(SpecialType.StdNumericsUInt8, SpecialType.StdNumericsUInt16) => ConversionKind.ImplicitNumeric,
			(SpecialType.StdNumericsUInt8, SpecialType.StdNumericsUInt32) => ConversionKind.ImplicitNumeric,
			(SpecialType.StdNumericsUInt8, SpecialType.StdNumericsUInt64) => ConversionKind.ImplicitNumeric,
			(SpecialType.StdNumericsUInt8, SpecialType.StdNumericsUNativeInt) => ConversionKind.ImplicitNumeric,
			(SpecialType.StdNumericsUInt16, SpecialType.StdNumericsUInt32) => ConversionKind.ImplicitNumeric,
			(SpecialType.StdNumericsUInt16, SpecialType.StdNumericsUInt64) => ConversionKind.ImplicitNumeric,
			(SpecialType.StdNumericsUInt16, SpecialType.StdNumericsUNativeInt) => ConversionKind.ImplicitNumeric,
			(SpecialType.StdNumericsUInt32, SpecialType.StdNumericsUInt64) => ConversionKind.ImplicitNumeric,
			(SpecialType.StdNumericsUInt32, SpecialType.StdNumericsUNativeInt) => ConversionKind.ImplicitNumeric,

			// Implicit: float extension
			(SpecialType.StdNumericsFloat16, SpecialType.StdNumericsFloat32) => ConversionKind.ImplicitFloatExtension,
			(SpecialType.StdNumericsFloat16, SpecialType.StdNumericsFloat64) => ConversionKind.ImplicitFloatExtension,
			(SpecialType.StdNumericsFloat32, SpecialType.StdNumericsFloat64) => ConversionKind.ImplicitFloatExtension,

			// Implicit: signed integer to float
			(SpecialType.StdNumericsSInt8, SpecialType.StdNumericsFloat16) => ConversionKind.ImplicitSignedIntegerToFloat,
			(SpecialType.StdNumericsSInt8, SpecialType.StdNumericsFloat32) => ConversionKind.ImplicitSignedIntegerToFloat,
			(SpecialType.StdNumericsSInt8, SpecialType.StdNumericsFloat64) => ConversionKind.ImplicitSignedIntegerToFloat,
			(SpecialType.StdNumericsSInt16, SpecialType.StdNumericsFloat16) => ConversionKind.ImplicitSignedIntegerToFloat,
			(SpecialType.StdNumericsSInt16, SpecialType.StdNumericsFloat32) => ConversionKind.ImplicitSignedIntegerToFloat,
			(SpecialType.StdNumericsSInt16, SpecialType.StdNumericsFloat64) => ConversionKind.ImplicitSignedIntegerToFloat,
			(SpecialType.StdNumericsSInt32, SpecialType.StdNumericsFloat16) => ConversionKind.ImplicitSignedIntegerToFloat,
			(SpecialType.StdNumericsSInt32, SpecialType.StdNumericsFloat32) => ConversionKind.ImplicitSignedIntegerToFloat,
			(SpecialType.StdNumericsSInt32, SpecialType.StdNumericsFloat64) => ConversionKind.ImplicitSignedIntegerToFloat,
			(SpecialType.StdNumericsSInt64, SpecialType.StdNumericsFloat16) => ConversionKind.ImplicitSignedIntegerToFloat,
			(SpecialType.StdNumericsSInt64, SpecialType.StdNumericsFloat32) => ConversionKind.ImplicitSignedIntegerToFloat,
			(SpecialType.StdNumericsSInt64, SpecialType.StdNumericsFloat64) => ConversionKind.ImplicitSignedIntegerToFloat,
			(SpecialType.StdNumericsSNativeInt, SpecialType.StdNumericsFloat16) => ConversionKind.ImplicitSignedIntegerToFloat,
			(SpecialType.StdNumericsSNativeInt, SpecialType.StdNumericsFloat32) => ConversionKind.ImplicitSignedIntegerToFloat,
			(SpecialType.StdNumericsSNativeInt, SpecialType.StdNumericsFloat64) => ConversionKind.ImplicitSignedIntegerToFloat,

			// Explicit: signed integer narrowing
			(SpecialType.StdNumericsSInt16, SpecialType.StdNumericsSInt8) => ConversionKind.ExplicitNumericTruncate,
			(SpecialType.StdNumericsSInt32, SpecialType.StdNumericsSInt8) => ConversionKind.ExplicitNumericTruncate,
			(SpecialType.StdNumericsSInt32, SpecialType.StdNumericsSInt16) => ConversionKind.ExplicitNumericTruncate,
			(SpecialType.StdNumericsSInt64, SpecialType.StdNumericsSInt8) => ConversionKind.ExplicitNumericTruncate,
			(SpecialType.StdNumericsSInt64, SpecialType.StdNumericsSInt16) => ConversionKind.ExplicitNumericTruncate,
			(SpecialType.StdNumericsSInt64, SpecialType.StdNumericsSInt32) => ConversionKind.ExplicitNumericTruncate,
			(SpecialType.StdNumericsSNativeInt, SpecialType.StdNumericsSInt8) => ConversionKind.ExplicitNumericTruncate,
			(SpecialType.StdNumericsSNativeInt, SpecialType.StdNumericsSInt16) => ConversionKind.ExplicitNumericTruncate,
			(SpecialType.StdNumericsSNativeInt, SpecialType.StdNumericsSInt32) => ConversionKind.ExplicitNumericTruncate,
			(SpecialType.StdNumericsSNativeInt, SpecialType.StdNumericsSInt64) => ConversionKind.ExplicitNumericTruncate,

			// Explicit: unsigned integer narrowing
			(SpecialType.StdNumericsUInt16, SpecialType.StdNumericsUInt8) => ConversionKind.ExplicitNumericTruncate,
			(SpecialType.StdNumericsUInt32, SpecialType.StdNumericsUInt8) => ConversionKind.ExplicitNumericTruncate,
			(SpecialType.StdNumericsUInt32, SpecialType.StdNumericsUInt16) => ConversionKind.ExplicitNumericTruncate,
			(SpecialType.StdNumericsUInt64, SpecialType.StdNumericsUInt8) => ConversionKind.ExplicitNumericTruncate,
			(SpecialType.StdNumericsUInt64, SpecialType.StdNumericsUInt16) => ConversionKind.ExplicitNumericTruncate,
			(SpecialType.StdNumericsUInt64, SpecialType.StdNumericsUInt32) => ConversionKind.ExplicitNumericTruncate,
			(SpecialType.StdNumericsUNativeInt, SpecialType.StdNumericsUInt8) => ConversionKind.ExplicitNumericTruncate,
			(SpecialType.StdNumericsUNativeInt, SpecialType.StdNumericsUInt16) => ConversionKind.ExplicitNumericTruncate,
			(SpecialType.StdNumericsUNativeInt, SpecialType.StdNumericsUInt32) => ConversionKind.ExplicitNumericTruncate,
			(SpecialType.StdNumericsUNativeInt, SpecialType.StdNumericsUInt64) => ConversionKind.ExplicitNumericTruncate,

			// Explicit: unsigned to wider signed (zero extend)
			(SpecialType.StdNumericsUInt8, SpecialType.StdNumericsSInt16) => ConversionKind.ExplicitNumericZExt,
			(SpecialType.StdNumericsUInt8, SpecialType.StdNumericsSInt32) => ConversionKind.ExplicitNumericZExt,
			(SpecialType.StdNumericsUInt8, SpecialType.StdNumericsSInt64) => ConversionKind.ExplicitNumericZExt,
			(SpecialType.StdNumericsUInt8, SpecialType.StdNumericsSNativeInt) => ConversionKind.ExplicitNumericZExt,
			(SpecialType.StdNumericsUInt16, SpecialType.StdNumericsSInt32) => ConversionKind.ExplicitNumericZExt,
			(SpecialType.StdNumericsUInt16, SpecialType.StdNumericsSInt64) => ConversionKind.ExplicitNumericZExt,
			(SpecialType.StdNumericsUInt16, SpecialType.StdNumericsSNativeInt) => ConversionKind.ExplicitNumericZExt,
			(SpecialType.StdNumericsUInt32, SpecialType.StdNumericsSInt64) => ConversionKind.ExplicitNumericZExt,
			(SpecialType.StdNumericsUInt32, SpecialType.StdNumericsSNativeInt) => ConversionKind.ExplicitNumericZExt,
			(SpecialType.StdNumericsUInt64, SpecialType.StdNumericsSNativeInt) => ConversionKind.ExplicitNumericZExt,

			// Explicit: signed to unsigned (same size)
			(SpecialType.StdNumericsUInt8, SpecialType.StdNumericsSInt8) => ConversionKind.ExplicitSignedToUnsigned,
			(SpecialType.StdNumericsUInt16, SpecialType.StdNumericsSInt16) => ConversionKind.ExplicitSignedToUnsigned,
			(SpecialType.StdNumericsUInt32, SpecialType.StdNumericsSInt32) => ConversionKind.ExplicitSignedToUnsigned,
			(SpecialType.StdNumericsUInt64, SpecialType.StdNumericsSInt64) => ConversionKind.ExplicitSignedToUnsigned,
			(SpecialType.StdNumericsUNativeInt, SpecialType.StdNumericsSNativeInt) => ConversionKind.ExplicitSignedToUnsigned,
			(SpecialType.StdNumericsSInt8, SpecialType.StdNumericsUInt8) => ConversionKind.ExplicitUnsignedToSigned,
			(SpecialType.StdNumericsSInt16, SpecialType.StdNumericsUInt16) => ConversionKind.ExplicitUnsignedToSigned,
			(SpecialType.StdNumericsSInt32, SpecialType.StdNumericsUInt32) => ConversionKind.ExplicitUnsignedToSigned,
			(SpecialType.StdNumericsSInt64, SpecialType.StdNumericsUInt64) => ConversionKind.ExplicitUnsignedToSigned,
			(SpecialType.StdNumericsSNativeInt, SpecialType.StdNumericsUNativeInt) => ConversionKind.ExplicitUnsignedToSigned,

			// Explicit: unsigned to float
			(SpecialType.StdNumericsUInt8, SpecialType.StdNumericsFloat16) => ConversionKind.ExplicitUnsignedIntegerToFloat,
			(SpecialType.StdNumericsUInt8, SpecialType.StdNumericsFloat32) => ConversionKind.ExplicitUnsignedIntegerToFloat,
			(SpecialType.StdNumericsUInt8, SpecialType.StdNumericsFloat64) => ConversionKind.ExplicitUnsignedIntegerToFloat,
			(SpecialType.StdNumericsUInt16, SpecialType.StdNumericsFloat16) => ConversionKind.ExplicitUnsignedIntegerToFloat,
			(SpecialType.StdNumericsUInt16, SpecialType.StdNumericsFloat32) => ConversionKind.ExplicitUnsignedIntegerToFloat,
			(SpecialType.StdNumericsUInt16, SpecialType.StdNumericsFloat64) => ConversionKind.ExplicitUnsignedIntegerToFloat,
			(SpecialType.StdNumericsUInt32, SpecialType.StdNumericsFloat16) => ConversionKind.ExplicitUnsignedIntegerToFloat,
			(SpecialType.StdNumericsUInt32, SpecialType.StdNumericsFloat32) => ConversionKind.ExplicitUnsignedIntegerToFloat,
			(SpecialType.StdNumericsUInt32, SpecialType.StdNumericsFloat64) => ConversionKind.ExplicitUnsignedIntegerToFloat,
			(SpecialType.StdNumericsUInt64, SpecialType.StdNumericsFloat16) => ConversionKind.ExplicitUnsignedIntegerToFloat,
			(SpecialType.StdNumericsUInt64, SpecialType.StdNumericsFloat32) => ConversionKind.ExplicitUnsignedIntegerToFloat,
			(SpecialType.StdNumericsUInt64, SpecialType.StdNumericsFloat64) => ConversionKind.ExplicitUnsignedIntegerToFloat,
			(SpecialType.StdNumericsUNativeInt, SpecialType.StdNumericsFloat16) => ConversionKind.ExplicitUnsignedIntegerToFloat,
			(SpecialType.StdNumericsUNativeInt, SpecialType.StdNumericsFloat32) => ConversionKind.ExplicitUnsignedIntegerToFloat,
			(SpecialType.StdNumericsUNativeInt, SpecialType.StdNumericsFloat64) => ConversionKind.ExplicitUnsignedIntegerToFloat,

			// Explicit: float to integer
			(SpecialType.StdNumericsFloat16, SpecialType.StdNumericsSInt8) => ConversionKind.ExplicitFloatToInteger,
			(SpecialType.StdNumericsFloat16, SpecialType.StdNumericsSInt16) => ConversionKind.ExplicitFloatToInteger,
			(SpecialType.StdNumericsFloat16, SpecialType.StdNumericsSInt32) => ConversionKind.ExplicitFloatToInteger,
			(SpecialType.StdNumericsFloat16, SpecialType.StdNumericsSInt64) => ConversionKind.ExplicitFloatToInteger,
			(SpecialType.StdNumericsFloat16, SpecialType.StdNumericsSNativeInt) => ConversionKind.ExplicitFloatToInteger,
			(SpecialType.StdNumericsFloat16, SpecialType.StdNumericsUInt8) => ConversionKind.ExplicitFloatToInteger,
			(SpecialType.StdNumericsFloat16, SpecialType.StdNumericsUInt16) => ConversionKind.ExplicitFloatToInteger,
			(SpecialType.StdNumericsFloat16, SpecialType.StdNumericsUInt32) => ConversionKind.ExplicitFloatToInteger,
			(SpecialType.StdNumericsFloat16, SpecialType.StdNumericsUInt64) => ConversionKind.ExplicitFloatToInteger,
			(SpecialType.StdNumericsFloat16, SpecialType.StdNumericsUNativeInt) => ConversionKind.ExplicitFloatToInteger,
			(SpecialType.StdNumericsFloat32, SpecialType.StdNumericsSInt8) => ConversionKind.ExplicitFloatToInteger,
			(SpecialType.StdNumericsFloat32, SpecialType.StdNumericsSInt16) => ConversionKind.ExplicitFloatToInteger,
			(SpecialType.StdNumericsFloat32, SpecialType.StdNumericsSInt32) => ConversionKind.ExplicitFloatToInteger,
			(SpecialType.StdNumericsFloat32, SpecialType.StdNumericsSInt64) => ConversionKind.ExplicitFloatToInteger,
			(SpecialType.StdNumericsFloat32, SpecialType.StdNumericsSNativeInt) => ConversionKind.ExplicitFloatToInteger,
			(SpecialType.StdNumericsFloat32, SpecialType.StdNumericsUInt8) => ConversionKind.ExplicitFloatToInteger,
			(SpecialType.StdNumericsFloat32, SpecialType.StdNumericsUInt16) => ConversionKind.ExplicitFloatToInteger,
			(SpecialType.StdNumericsFloat32, SpecialType.StdNumericsUInt32) => ConversionKind.ExplicitFloatToInteger,
			(SpecialType.StdNumericsFloat32, SpecialType.StdNumericsUInt64) => ConversionKind.ExplicitFloatToInteger,
			(SpecialType.StdNumericsFloat32, SpecialType.StdNumericsUNativeInt) => ConversionKind.ExplicitFloatToInteger,
			(SpecialType.StdNumericsFloat64, SpecialType.StdNumericsSInt8) => ConversionKind.ExplicitFloatToInteger,
			(SpecialType.StdNumericsFloat64, SpecialType.StdNumericsSInt16) => ConversionKind.ExplicitFloatToInteger,
			(SpecialType.StdNumericsFloat64, SpecialType.StdNumericsSInt32) => ConversionKind.ExplicitFloatToInteger,
			(SpecialType.StdNumericsFloat64, SpecialType.StdNumericsSInt64) => ConversionKind.ExplicitFloatToInteger,
			(SpecialType.StdNumericsFloat64, SpecialType.StdNumericsSNativeInt) => ConversionKind.ExplicitFloatToInteger,
			(SpecialType.StdNumericsFloat64, SpecialType.StdNumericsUInt8) => ConversionKind.ExplicitFloatToInteger,
			(SpecialType.StdNumericsFloat64, SpecialType.StdNumericsUInt16) => ConversionKind.ExplicitFloatToInteger,
			(SpecialType.StdNumericsFloat64, SpecialType.StdNumericsUInt32) => ConversionKind.ExplicitFloatToInteger,
			(SpecialType.StdNumericsFloat64, SpecialType.StdNumericsUInt64) => ConversionKind.ExplicitFloatToInteger,
			(SpecialType.StdNumericsFloat64, SpecialType.StdNumericsUNativeInt) => ConversionKind.ExplicitFloatToInteger,

			// Explicit: float truncation
			(SpecialType.StdNumericsFloat32, SpecialType.StdNumericsFloat16) => ConversionKind.ExplicitFloatTruncate,
			(SpecialType.StdNumericsFloat64, SpecialType.StdNumericsFloat16) => ConversionKind.ExplicitFloatTruncate,
			(SpecialType.StdNumericsFloat64, SpecialType.StdNumericsFloat32) => ConversionKind.ExplicitFloatTruncate,

			_ => ConversionKind.NoConversion,
		};
	}

	public static bool HasImplicitConversion(TypeSymbol fromType, TypeSymbol toType)
	{
		ConversionKind kind = ClassifyConversion(fromType, toType);
		return kind != ConversionKind.NoConversion && kind.IsImplicit;
	}
}
