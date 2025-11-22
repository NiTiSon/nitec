namespace NiteCompiler.CodeAnalysis.Syntax;

public partial class NiteLexer
{
	internal ref struct TokenInfo
	{
		public SyntaxKind Kind;
		public SyntaxKind ContextualKind;
		public NumericLiteralFormat LiteralFormat;
		public NumericLiteralType LiteralType;
	}
}

public enum NumericLiteralFormat : byte
{
	Integer,
	Float,
	ENotation,
}

public enum NumericLiteralType : byte
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