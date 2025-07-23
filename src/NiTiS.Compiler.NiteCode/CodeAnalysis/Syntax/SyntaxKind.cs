using static NiTiS.Compiler.NiteCode.CodeAnalysis.Syntax.SyntaxKindExtensions;

namespace NiTiS.Compiler.NiteCode.CodeAnalysis.Syntax;

public enum SyntaxKind : uint
{
	Invalid = 0,
	EndOfFile = 0xFFFFFFFFu,

	// === TRIVIA ===
	WhitespaceTrivia = TriviaFlag + 1,
	LineBreakTrivia = TriviaFlag + 2,
	SingleLineCommentTrivia = TriviaFlag + 3,
	MultiLineCommentTrivia = TriviaFlag + 4,

	// === Tokens ===
	/// <summary>
	/// Identifier token.
	/// </summary>
	Identifier = ValuableFlag + 101,
	/// <summary>
	/// Number token.
	/// </summary>
	NumberToken = ValuableFlag + 102,

	/// <summary>Represents <c>+</c> token.</summary>
	PlusToken = OperatorFlag + 103,
	/// <summary>Represents <c>-</c> token.</summary>
	MinusToken = OperatorFlag + 104,
	/// <summary>Represents <c>!</c> token.</summary>
	ExclamationToken = OperatorFlag + 105,
	/// <summary>Represents <c>"</c> token.</summary>
	DoubleQuote = 106,
	/// <summary>Represents <c>#</c> token.</summary>
	HashToken = 107,
	/// <summary>Represents <c>$</c> token.</summary>
	DollarToken = 108,
	/// <summary>Represents <c>%</c> token.</summary>
	PercentToken = OperatorFlag + 109,
	/// <summary>Represents <c>&amp;</c> token.</summary>
	AmpersandToken = OperatorFlag + 110,
	/// <summary>Represents <c>'</c> token.</summary>
	QuoteToken = 111,
	/// <summary>Represents <c>(</c> token.</summary>
	OpenParenToken = 113,
	/// <summary>Represents <c>)</c> token.</summary>
	CloseParenToken = 114,
	/// <summary>Represents <c>*</c> token.</summary>
	AsteriskToken = OperatorFlag + 115,
	/// <summary>Represents <c>,</c> token.</summary>
	CommaToken = 116,
	/// <summary>Represents <c>.</c> token.</summary>
	DotToken = 117,
	/// <summary>Represents <c>/</c> token.</summary>
	SlashToken = OperatorFlag + 118,
	// SKIPPED ASCII NUMBERS
	/// <summary>Represents <c>:</c> token.</summary>
	ColonToken = 129,
	/// <summary>Represents <c>;</c> token.</summary>
	SemicolonToken = 130,
	/// <summary>Represents <c>&lt;</c> token.</summary>
	LessThanToken = OperatorFlag + 131,
	/// <summary>Represents <c>=</c> token.</summary>
	EqualsToken = OperatorFlag + 132,
	/// <summary>Represents <c>&gt;</c> token.</summary>
	GreaterThanToken = OperatorFlag + 133,
	/// <summary>Represents <c>?</c> token.</summary>
	QuestionToken = 134,
	/// <summary>Represents <c>@</c> token.</summary>
	AtToken = 135,
	// SKIPPED ASCII UPPERCASE LETTERS
	/// <summary>Represents <c>[</c> token.</summary>
	OpenBracketToken = 169,
	/// <summary>Represents <c>\</c> token.</summary>
	BackslashToken = 170,
	/// <summary>Represents <c>]</c> token.</summary>
	CloseBracketToken = 171,
	/// <summary>Represents <c>^</c> token.</summary>
	CaretToken = OperatorFlag + 139,
	/// <inheritdoc cref="CaretToken"/>
	CircumflexToken = CaretToken,
	/// <summary>Represents <c>_</c> token.</summary>
	UnderscoreToken = 140,
	/// <summary>Represents <c>`</c> token.</summary>
	BacktickToken = 141,
	// SKIPPED ASCII LOWERCASE LETTERS
	/// <summary>Represents <c>{</c> token.</summary>
	OpenBraceToken = 175,
	/// <summary>Represents <c>|</c> token.</summary>
	PipeToken = OperatorFlag + 176,
	/// <summary>Represents <c>}</c> token.</summary>
	CloseBraceToken = 177,
	/// <summary>Represents <c>~</c> token.</summary>
	TildeToken = OperatorFlag + 178,
	/// <summary>Represents <c>--</c> token.</summary>
	MinusMinusToken = OperatorFlag + 179,
	/// <summary>Represents <c>++</c> token.</summary>
	PlusPlusToken = OperatorFlag + 180,
	/// <summary>Represents <c>+=</c> token.</summary>
	PlusEqualsToken = OperatorFlag + 181,
	/// <summary>Represents <c>-=</c> token.</summary>
	MinusEqualsToken = OperatorFlag + 182,
	/// <summary>Represents <c>*=</c> token.</summary>
	AsteriskEqualsToken = OperatorFlag + 183,
	/// <summary>Represents <c>/=</c> token.</summary>
	SlashEqualsToken = OperatorFlag + 184,
	/// <summary>Represents <c>%=</c> token.</summary>
	PercentEqualsToken = OperatorFlag + 185,
	/// <summary>Represents <c>&=</c> token.</summary>
	AmpersandEqualsToken = OperatorFlag + 186,
	/// <summary>Represents <c>|=</c> token.</summary>
	PipeEqualsToken = OperatorFlag + 187,
	/// <summary>Represents <c>^=</c> token.</summary>
	CaretEqualsToken = OperatorFlag + 188,
	/// <inheritdoc cref="CaretEqualsToken"/>
	CircumflexEqualsToken = CaretEqualsToken,
	/// <summary>Represents <c>&lt;&lt;</c> token.</summary>
	LeftShiftToken = OperatorFlag + 189,
	/// <summary>Represents <c>&gt;&gt;</c> token.</summary>
	RightShiftToken = OperatorFlag + 190,
	/// <summary>Represents <c>&gt;&gt;&gt;</c> token.</summary>
	UnsignedRightShiftToken = OperatorFlag + 191,
	/// <summary>Represents <c>&lt;&lt;=</c> token.</summary>
	LeftShiftEqualsToken = OperatorFlag + 192,
	/// <summary>Represents <c>&gt;&gt;=</c> token.</summary>
	RightShiftEqualsToken = OperatorFlag + 193,
	/// <summary>Represents <c>&gt;&gt;&gt;=</c> token.</summary>
	UnsignedRightShiftEqualsToken = OperatorFlag + 194,

	// === Keywords ===
	TrueKeyword = KeywordFlag + 301,
	FalseKeyword = KeywordFlag + 302,
	UseKeyword = KeywordFlag + 303,
	IfKeyword = KeywordFlag + 304,
	ElseKeyword = KeywordFlag + 305,
	LoopKeyword = KeywordFlag + 306,
	ForKeyword = KeywordFlag + 307,
	DoKeyword = KeywordFlag + 308,
	WhileKeyword = KeywordFlag + 309,
	BreakKeyword = KeywordFlag + 310,
	ReturnKeyword = KeywordFlag + 311,
	ContinueKeyword = KeywordFlag + 312,
	GetKeyword = ContextualFlag + KeywordFlag + 313,
	SetKeyword = ContextualFlag + KeywordFlag + 314,
	WhereKeyword = ContextualFlag + KeywordFlag + 315,
	WhenKeyword = ContextualFlag + KeywordFlag + 316,
	IsKeyword = KeywordFlag + 317,
	AsKeyword = KeywordFlag + 318,
	OperatorKeyword = ContextualFlag + KeywordFlag + 318,
	CommutativeKeyword = ContextualFlag + KeywordFlag + 319,
	PublicKeyword = KeywordFlag + 320,
	FriendKeyword = KeywordFlag + 321,
	ProtectedKeyword = KeywordFlag + 322,
	InternalKeyword = KeywordFlag + 323,
	FamilyKeyword = KeywordFlag + 324,
	PrivateKeyword = KeywordFlag + 325,

	// === TYPE KEYWORDS ===
	I8Keyword = TypeFlag + KeywordFlag + 401,
	I16Keyword = TypeFlag + KeywordFlag + 402,
	I32Keyword = TypeFlag + KeywordFlag + 403,
	I64Keyword = TypeFlag + KeywordFlag + 404,
	U8Keyword = TypeFlag + KeywordFlag + 405,
	U16Keyword = TypeFlag + KeywordFlag + 406,
	U32Keyword = TypeFlag + KeywordFlag + 407,
	U64Keyword = TypeFlag + KeywordFlag + 408,
	F16Keyword = TypeFlag + KeywordFlag + 409,
	F32Keyword = TypeFlag + KeywordFlag + 410,
	F64Keyword = TypeFlag + KeywordFlag + 411,
	VoidKeyword = TypeFlag + KeywordFlag + 412,
	BoolKeyword = TypeFlag + KeywordFlag + 413,
}