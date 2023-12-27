namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public enum NumberLiteralNumeralSystem
{
	Decimal,			// 1201203120313091283128
	Hexadecimal,		// 0xFF13A
	Binary,				// 0b010010100
}
public enum NumberLiteralType
{
	Auto,
	SignedInt,
	UnsignedInt,
	U8,
	U16,
	U32,
	U64,
	U128,
	I8,
	I16,
	I32,
	I64,
	I128,
	F16,
	F32,
	F64,
	F128,
}