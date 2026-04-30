using System.Collections.Frozen;
using System.Collections.Generic;

namespace NiteCompiler;

public enum SpecialType : byte
{
	None = 0,
	StdNumericsSInt8 = 1,
	StdNumericsSInt16 = 2,
	StdNumericsSInt32 = 3,
	StdNumericsSInt64 = 4,
	StdNumericsSNativeInt = 5,
	StdNumericsUInt8 = 6,
	StdNumericsUInt16 = 7,
	StdNumericsUInt32 = 8,
	StdNumericsUInt64 = 9,
	StdNumericsUNativeInt = 10,
	StdNumericsFloat16 = 11,
	StdNumericsFloat32 = 12,
	StdNumericsFloat64 = 13,
	StdVoid = 14,
	StdNeverReturn = 15,
	StdBoolean = 16,
	StdTextCharacterUtf8 = 17,
	StdTextCharacterUtf16 = 18,
	StdTextCharacterUtf32 = 19,
	// Change SpecialTypeExtensions.Count if add new special types
}

internal static class SpecialTypeExtensions
{
	private static readonly FrozenDictionary<string, SpecialType> Map;
	private static readonly string[] Names = [
		null!,
		"std::numerics::SInt8",
		"std::numerics::SInt16",
		"std::numerics::SInt32",
		"std::numerics::SInt64",
		"std::numerics::SNativeInt",
		"std::numerics::UInt8",
		"std::numerics::UInt16",
		"std::numerics::UInt32",
		"std::numerics::UInt64",
		"std::numerics::UNativeInt",
		"std::numerics::Float16",
		"std::numerics::Float32",
		"std::numerics::Float64",
		"std::Void",
		"std::NeverReturn",
		"std::Boolean",
		"std::text::CharacterUtf8",
		"std::text::CharacterUtf16",
		"std::text::CharacterUtf32",
	];

	static SpecialTypeExtensions()
	{
		Dictionary<string, SpecialType> dict = [];
		for (int i = 1; i < (int)SpecialType.Count; i++)
		{
			dict[Names[i]] = (SpecialType)i;
		}

		Map = dict.ToFrozenDictionary();
	}

	extension(SpecialType self)
	{
		public static SpecialType Count => SpecialType.StdTextCharacterUtf32 + 1;

		public static SpecialType GetSpecialTypeFromFullName(string fullName)
		{
			Map.TryGetValue(fullName, out SpecialType result);
			return result;
		}

		public string ToFullName()
		{
			return (string?)Names[(int)self]!;
		}

		public bool IsSignedIntegral =>
			self is SpecialType.StdNumericsSInt8
				or SpecialType.StdNumericsSInt16
				or SpecialType.StdNumericsSInt32
				or SpecialType.StdNumericsSInt64
				or SpecialType.StdNumericsSNativeInt;

		public bool IsUnsignedIntegral =>
			self is SpecialType.StdNumericsUInt8
				or SpecialType.StdNumericsUInt16
				or SpecialType.StdNumericsUInt32
				or SpecialType.StdNumericsUInt64
				or SpecialType.StdNumericsUNativeInt;

		public bool IsFloat =>
			self is SpecialType.StdNumericsFloat16
				or SpecialType.StdNumericsFloat32
				or SpecialType.StdNumericsFloat64;

		public bool IsCharacter =>
			self is SpecialType.StdTextCharacterUtf8
				or SpecialType.StdTextCharacterUtf16
				or SpecialType.StdTextCharacterUtf32;
	}
}