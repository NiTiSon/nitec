namespace NiteCompiler.CodeAnalysis.Syntax;

public enum SyntaxKind : uint
{
	Invalid = 0,
	EndOfFile = 0xFFFFFFFFu,
	CompilationUnit = 1,

	// === Tokens ===
	/// <summary>
	/// Identifier token.
	/// </summary>
	IdentifierToken = 101,
	/// <summary>
	/// Number token.
	/// </summary>
	NumberToken = 102,

	/// <summary>Represents <c>+</c> token.</summary>
	PlusToken = 103,
	/// <summary>Represents <c>-</c> token.</summary>
	MinusToken = 104,
	/// <summary>Represents <c>!</c> token.</summary>
	ExclamationToken = 105,
	/// <summary>Represents <c>"</c> token.</summary>
	DoubleQuote = 106,
	/// <summary>Represents <c>#</c> token.</summary>
	HashToken = 107,
	/// <summary>Represents <c>$</c> token.</summary>
	DollarToken = 108,
	/// <summary>Represents <c>%</c> token.</summary>
	PercentToken = 109,
	/// <summary>Represents <c>&amp;</c> token.</summary>
	AmpersandToken = 110,
	/// <summary>Represents <c>'</c> token.</summary>
	QuoteToken = 111,
	/// <summary>Represents <c>(</c> token.</summary>
	OpenParenToken = 113,
	/// <summary>Represents <c>)</c> token.</summary>
	CloseParenToken = 114,
	/// <summary>Represents <c>*</c> token.</summary>
	AsteriskToken = 115,
	/// <summary>Represents <c>,</c> token.</summary>
	CommaToken = 116,
	/// <summary>Represents <c>.</c> token.</summary>
	DotToken = 117,
	/// <summary>Represents <c>/</c> token.</summary>
	SlashToken = 118,
	// SKIPPED ASCII NUMBERS
	/// <summary>Represents <c>:</c> token.</summary>
	ColonToken = 129,
	/// <summary>Represents <c>;</c> token.</summary>
	SemicolonToken = 130,
	/// <summary>Represents <c>&lt;</c> token.</summary>
	LessThanToken = 131,
	/// <summary>Represents <c>=</c> token.</summary>
	EqualsToken = 132,
	/// <summary>Represents <c>&gt;</c> token.</summary>
	GreaterThanToken = 133,
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
	CaretToken = 139,
	/// <inheritdoc cref="CaretToken"/>
	CircumflexToken = CaretToken,
	/// <summary>Represents <c>_</c> token.</summary>
	UnderscoreToken = 140,
	// BacktickToken REMOVED: NOT USED
	// SKIPPED ASCII LOWERCASE LETTERS
	/// <summary>Represents <c>{</c> token.</summary>
	OpenBraceToken = 175,
	/// <summary>Represents <c>|</c> token.</summary>
	PipeToken = 176,
	/// <summary>Represents <c>}</c> token.</summary>
	CloseBraceToken = 177,
	/// <summary>Represents <c>~</c> token.</summary>
	TildeToken = 178,
	/// <summary>Represents <c>--</c> token.</summary>
	MinusMinusToken = 179,
	/// <summary>Represents <c>++</c> token.</summary>
	PlusPlusToken = 180,
	/// <summary>Represents <c>+=</c> token.</summary>
	PlusEqualsToken = 181,
	/// <summary>Represents <c>-=</c> token.</summary>
	MinusEqualsToken = 182,
	/// <summary>Represents <c>*=</c> token.</summary>
	AsteriskEqualsToken = 183,
	/// <summary>Represents <c>/=</c> token.</summary>
	SlashEqualsToken = 184,
	/// <summary>Represents <c>%=</c> token.</summary>
	PercentEqualsToken = 185,
	/// <summary>Represents <c>&=</c> token.</summary>
	AmpersandEqualsToken = 186,
	/// <summary>Represents <c>|=</c> token.</summary>
	PipeEqualsToken = 187,
	/// <summary>Represents <c>^=</c> token.</summary>
	CaretEqualsToken = 188,
	/// <inheritdoc cref="CaretEqualsToken"/>
	CircumflexEqualsToken = CaretEqualsToken,
	/// <summary>Represents <c>&lt;&lt;</c> token.</summary>
	LeftShiftToken = 189,
	/// <summary>Represents <c>&gt;&gt;</c> token.</summary>
	RightShiftToken = 190,
	/// <summary>Represents <c>&gt;&gt;&gt;</c> token.</summary>
	UnsignedRightShiftToken = 191,
	/// <summary>Represents <c>&lt;&lt;=</c> token.</summary>
	LeftShiftEqualsToken = 192,
	/// <summary>Represents <c>&gt;&gt;=</c> token.</summary>
	RightShiftEqualsToken = 193,
	/// <summary>Represents <c>&gt;&gt;&gt;=</c> token.</summary>
	UnsignedRightShiftEqualsToken = 194,
	/// <summary>Represents <c>==</c> token.</summary>
	EqualsEqualsToken = 195,
	/// <summary>Represents <c>-&gt;</c> token.</summary>
	RetusaToken = 196,

	/// <summary>Represents <c>::</c> token.</summary>
	ColonColonToken = 250,
	/// <summary>Represents <c>..</c> token.</summary>
	DotDotToken = 251,
	/// <summary>Represents <c>..=</c> token.</summary>
	DotDotEqualsToken = 252,

	// === Keywords ===
	TrueKeyword = 301,
	FalseKeyword = 302,
	UseKeyword = 303,
	IfKeyword = 304,
	ElseKeyword = 305,
	LoopKeyword = 306,
	ForKeyword = 307,
	DoKeyword = 308,
	WhileKeyword = 309,
	BreakKeyword = 310,
	ReturnKeyword = 311,
	ContinueKeyword = 312,
	GetKeyword = 313,
	SetKeyword = 314,
	WhereKeyword = 315,
	WhenKeyword = 316,
	IsKeyword = 317,
	AsKeyword = 318,
	OperatorKeyword = 319,
	CommutativeKeyword = 320,
	PublicKeyword = 321,
	FriendKeyword = 322,
	ProtectedKeyword = 323,
	InternalKeyword = 324,
	FamilyKeyword = 325,
	PrivateKeyword = 326,
	ModuleKeyword = 327,
	LetKeyword = 328,

	// === TYPE KEYWORDS ===
	I8Keyword = 401,
	I16Keyword = 402,
	I32Keyword = 403,
	I64Keyword = 404,
	U8Keyword = 405,
	U16Keyword = 406,
	U32Keyword = 407,
	U64Keyword = 408,
	F16Keyword = 409,
	F32Keyword = 410,
	F64Keyword = 411,
	VoidKeyword = 412,
	BoolKeyword = 413,

	// === STATEMENTS ===
	BlockStatement = 2001,
	ReturnStatement = 2002,

	// === EXPRESSIONS ===
	BinaryExpression = 3001,
	UnaryExpression = 3002,
	LiteralExpression = 3003,

	// === NAME EXPRESSIONS ===
	IdentifierName = 3501, // id
	ModuleName = 3502, // id::id::id
	QualifiedName = 3503, // id::id::Member

	// === DECLARATIONS ===
	FunctionDeclaration = 4001,
	TypeDeclaration = 4002,
	VariableDeclaration = 4003,
	ModuleDeclaration = 4004,

	// === DIRECTIVE ===
	UseDirective = 5001,
	// UseAsDirective // use std::Type as OtherName;
	WhenDirective = 5002,
	WhereDirective = 5003,

	IdentifierList = 6001,
	ParameterList = 6002,
}