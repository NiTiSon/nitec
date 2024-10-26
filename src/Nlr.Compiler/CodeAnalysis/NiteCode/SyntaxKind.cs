namespace Nlr.Compiler.CodeAnalysis.NiteCode;

public enum SyntaxKind : ushort
{
	None = 0,

	EndOfFile = 1,

	// === Punctuation ===
	/// <summary>Represents <c>~</c> token.</summary>
	TildeToken = 1001,

	/// <summary>Represents <c>!</c> token.</summary>
	ExclamationToken = 1002,

	/// <summary>Represents <c>%</c> token.</summary>
	PercentToken = 1003,

	/// <summary>Represents <c>^</c> token.</summary>
	CaretToken = 1004,

	/// <summary>Represents <c>@</c> token.</summary>
	AtToken = 1005,

	/// <summary>Represents <c>&amp;</c> token.</summary>
	AmpersandToken = 1006,

	/// <summary>Represents <c>*</c> token.</summary>
	AsteriskToken = 1007,

	/// <summary>Represents <c>(</c> token.</summary>
	OpenParenToken = 1008,

	/// <summary>Represents <c>)</c> token.</summary>
	CloseParenToken = 1009,

	/// <summary>Represents <c>-</c> token.</summary>
	MinusToken = 1010,

	/// <summary>Represents <c>+</c> token.</summary>
	PlusToken = 1011,

	/// <summary>Represents <c>=</c> token.</summary>
	EqualsToken = 1012,

	/// <summary>Represents <c>{</c> token.</summary>
	OpenBraceToken = 1013,

	/// <summary>Represents <c>}</c> token.</summary>
	CloseBraceToken = 1014,

	/// <summary>Represents <c>[</c> token.</summary>
	OpenBracketToken = 1015,

	/// <summary>Represents <c>]</c> token.</summary>
	CloseBracketToken = 1016,

	/// <summary>Represents <c>|</c> token.</summary>
	BarToken = 1017,

	/// <summary>Represents <c>\</c> token.</summary>
	BackSlashToken = 1018,

	/// <summary>Represents <c>:</c> token.</summary>
	ColonToken = 1019,

	/// <summary>Represents <c>;</c> token.</summary>
	SemicolonToken = 1020,

	/// <summary>Represents <c>`</c> token.</summary>
	ApostropheToken = 1021,

	/// <summary>Represents <c>,</c> token.</summary>
	CommaToken = 1022,

	/// <summary>Represents <c>.</c> token.</summary>
	DotToken = 1023,

	/// <summary>Represents <c>..</c> token.</summary>
	DotDotToken = 1024,

	/// <summary>Represents <c>?</c> token.</summary>
	QuestionToken = 1025,

	/// <summary>Represents <c>/</c> token.</summary>
	SlashToken = 1026,

	/// <summary>Represents <c>::</c> token.</summary>
	ColonColonToken = 1051,

	// === Keywords ===
	/// <summary>Represents <see langword="bool"/>.</summary>
	BoolKeyword = 1101,

	/// <summary>Represents <see langword="u8"/>.</summary>
	U8Keyword = 1102,

	/// <summary>Represents <see langword="u16"/>.</summary>
	U16Keyword = 1103,

	/// <summary>Represents <see langword="u32"/>.</summary>
	U32Keyword = 1104,

	/// <summary>Represents <see langword="u64"/>.</summary>
	U64Keyword = 1105,

	/// <summary>Represents <see langword="i8"/>.</summary>
	I8Keyword = 1106,

	/// <summary>Represents <see langword="i16"/>.</summary>
	I16Keyword = 1107,

	/// <summary>Represents <see langword="i32"/>.</summary>
	I32Keyword = 1108,

	/// <summary>Represents <see langword="i64"/>.</summary>
	I64Keyword = 1109,

	/// <summary>Represents <see langword="f32"/>.</summary>
	F32Keyword = 1110,

	/// <summary>Represents <see langword="f64"/>.</summary>
	F64Keyword = 1111,

	/// <summary>Represents <see langword="void"/>.</summary>
	VoidKeyword = 1112,

	// === Function-like keywords ===
	/// <summary>Represents <see langword="typeof"/>.</summary>
	TypeOfKeyword = 1151,

	/// <summary>Represents <see langword="sizeof"/>.</summary>
	SizeOfKeyword = 1152,

	/// <summary>Represents <see langword="offsetof"/>.</summary>
	OffsetOfKeyword = 1153,

	/// <summary>Represents <see langword="default"/>.</summary>
	DefaultKeyword = 1154,

	// === Value keywords ===
	/// <summary>Represents <see langword="nil"/>.</summary>
	NilKeyword = 1161,

	/// <summary>Represents <see langword="true"/>.</summary>
	TrueKeyword = 1162,
	
	/// <summary>Represents <see langword="false"/>.</summary>
	FalseKeyword = 1163,

	// === Structure keywords ===
	/// <summary>Represents <see langword="use"/>.</summary>
	UseKeyword = 1201,

	/// <summary>Represents <see langword="module"/>.</summary>
	ModuleKeyword = 1202,

	// === Text ===
	IdentifierToken = 2001,
	NumericLiteralToken = 2002,
	BadToken = 2003,
	CharacterLiteralToken = 2004,
	//StringLiteralToken = 2005,
	//VerbatimStringLiteralToken = 2006,

	// === Trivia ===
	EndOfLineTrivia = 3001,
	WhitespaceTrivia = 3002,
	SingleLineComment = 3003,
	MultilineCommentTrivia = 3004,

	// === Declarations ===
	CompilationUnit = 55001,
	ModuleDeclaration = 55002,
	UseDirective = 55003,
}