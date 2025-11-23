namespace NiteLang.Metadata;

public enum SpecialType : byte
{
	None = 0,
	StdNumericsSInt8 = 1,
	StdNumericsSInt16 = 2,
	StdNumericsSInt32 = 3,
	StdNumericsSInt64 = 4,
	StdNumericsUInt8 = 5,
	StdNumericsUInt16 = 6,
	StdNumericsUInt32 = 7,
	StdNumericsUInt64 = 8,
	StdNumericsFloat16 = 9,
	StdNumericsFloat32 = 10,
	StdNumericsFloat64 = 11,
	StdVoid = 12,
	StdNeverReturn = 13,
	StdBoolean = 14,
}