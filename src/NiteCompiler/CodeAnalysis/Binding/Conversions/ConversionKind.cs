using System.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Binding.Conversions;

internal enum ConversionKind : byte
{
	NoConversion,
	Identity,

	ImplicitValueToOptional, // T -> T?
	ImplicitNumeric,         // i8 -> i16, u32 -> u64 (same signedness, wider)
	ImplicitFloatExtension,  // f16 -> f32 -> f64
	ImplicitSignedIntegerToFloat, // i32 -> f32
	ImplicitUserDefined,

	ExplicitNumericTruncate,       // i32 -> i16
	ExplicitNumericSExt,           // u32 -> i64 (smaller signed -> larger signed)
	ExplicitNumericZExt,           // u8 -> u16 (unsigned extension)
	ExplicitSignedToUnsigned,      // i32 -> u32 (same size, diff sign)
	ExplicitUnsignedToSigned,      // u32 -> i32 (same size, diff sign)
	ExplicitUnsignedIntegerToFloat, // u32 -> f32
	ExplicitFloatToInteger,        // f32 -> i32
	ExplicitFloatTruncate,         // f64 -> f32
	ExplicitUserDefined,
}

internal static class ConversionKindExtensions
{
	extension(ConversionKind kind)
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public bool IsImplicit
		{
			get
			{
				switch (kind)
				{
					case ConversionKind.NoConversion:
						return false;

					case ConversionKind.Identity:
					case ConversionKind.ImplicitValueToOptional:
					case ConversionKind.ImplicitNumeric:
					case ConversionKind.ImplicitFloatExtension:
					case ConversionKind.ImplicitSignedIntegerToFloat:
					case ConversionKind.ImplicitUserDefined:
						return true;

					case ConversionKind.ExplicitNumericTruncate:
					case ConversionKind.ExplicitNumericSExt:
					case ConversionKind.ExplicitNumericZExt:
					case ConversionKind.ExplicitSignedToUnsigned:
					case ConversionKind.ExplicitUnsignedToSigned:
					case ConversionKind.ExplicitUnsignedIntegerToFloat:
					case ConversionKind.ExplicitFloatToInteger:
					case ConversionKind.ExplicitFloatTruncate:
					case ConversionKind.ExplicitUserDefined:
						return false;

					default:
						throw new UnreachableException();
				}
			}
		}

		public bool IsUserDefined => kind is ConversionKind.ExplicitUserDefined or ConversionKind.ImplicitUserDefined;
	}
}
