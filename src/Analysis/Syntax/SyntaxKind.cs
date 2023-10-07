namespace NiteCompiler.Analysis.Syntax;

public enum SyntaxKind
{
	// Tokens:
	WrongToken,

	SkippedTextTrivia,
	LineBreakTrivia, // \r\n \n \r\n
	WhitespaceTrivia, // <SPACE>
	SingleLineCommentTrivia, // <- This
	MultiLineCommentTrivia, /* <- These -> */

	EndOfFile, // <EOF>

	NumberLiteral, // 0x321323i32 etc (by default i32)
	RealNumberLiteral, // 0.32f or 0.8123123123213f64 or 0.1 (by default f32+)
	StringLiteral, // "Some Text" (by default StringUtf32)
	BooleanLiteral, // true, false (always boolean)
	Identifier,

	Assign,
	Plus,
	PlusAssign,
	Minus,
	MinusAssign,
	Asterisk,
	AsteriskAssign,
	Slash,
	SlashAssign,

	Exclamation,
	Interrogation,
	Equals,
	NotEquals,
	Less,
	LessEquals,
	More,
	MoreEquals,
	Tilde,
	Hash,
	At,

	Or,
	FastOr,
	OrAssign,
	Xor,
	XorAssign,
	And,
	FastAnd,
	AndAssign,

	// Punctuation:
	OpenBrace,
	CloseBrace,
	OpenParen,
	CloseParen,
	OpenBracket,
	CloseBracket,
	Comma,
	Dot,
	Colon,
	DoubleColon,
	Semicolon,

	// Keyword:
	KwNew,
	KwDelete,

	KwPublic,
	KwProtected,
	KwPrivate,
	KwHomie,
	KwInternal,
	KwFamily,

	KwStatic,
	KwVirtual,
	KwAbstract,
	KwAlias,

	KwLimit,
	KwType,
	KwEnum,
	KwTrait,
	KwRef,
	KwLocal,
	KwReturn,
	//KwThrow,
	KwContinue,
	KwGoto,
	KwBreak,
	KwFor,
	KwIf,
	KwElse,
	KwWhile,
	KwDo,
	KwLoop,

	KwDefault, // use as default in Rust
	KwSizeOf,
	KwTypeOf,
	KwSelf,
	KwSelfType,

	// Types:
	KwVoid, // use as default in C# + void type
	KwI8,
	KwI16,
	KwI32,
	KwI64,
	KwU8,
	KwU16,
	KwU32,
	KwU64,
	KwF16,
	KwF32,
	KwF64,
	KwF128,
	KwChar, // Default char (utf16 by default, may be changed by compiler parameters)
	KwChar7, // ASCII
	KwChar8, // Utf8
	KwChar16, // Utf16
	KwChar32, // Utf32 (unicode point)

	KwNil, // aka null or zero
}