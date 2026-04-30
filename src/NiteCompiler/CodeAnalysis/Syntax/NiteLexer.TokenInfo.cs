namespace NiteCompiler.CodeAnalysis.Syntax;

internal partial class NiteLexer
{
	internal ref struct TokenInfo
	{
		public TokenKind Kind;
		public NumericLiteralFormat NumericFormat;
		public NumericLiteralType NumericType;
		public StringLiteralType StringType;
	}
}

internal enum NumericLiteralFormat : byte
{
	Integer,
	Float,
	ENotation,
}

internal enum NumericLiteralType : byte
{
	None,
	I8,
	I16,
	I32,
	I64,
	U8,
	U16,
	U32,
	U64,
	Signed,
	Unsigned,
	F16,
	F32,
	F64
}

internal enum StringLiteralType : byte
{
	None, // none explicitly -> utf-8
	Unicode8,
	Unicode16,
	Unicode32,
	Os
}