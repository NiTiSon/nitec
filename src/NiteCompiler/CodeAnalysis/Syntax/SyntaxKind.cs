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
	/// <summary>Represents <c>_</c> token.</summary>
	UnderscoreToken = 140,
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
	/// <summary>Represents <c>!=</c> token.</summary>
	ExclamationEqualsToken = 183,
	/// <summary>Represents <c>*=</c> token.</summary>
	AsteriskEqualsToken = 184,
	/// <summary>Represents <c>/=</c> token.</summary>
	SlashEqualsToken = 185,
	/// <summary>Represents <c>%=</c> token.</summary>
	PercentEqualsToken = 186,
	/// <summary>Represents <c>&amp;=</c> token.</summary>
	AmpersandEqualsToken = 187,
	/// <summary>Represents <c>|=</c> token.</summary>
	PipeEqualsToken = 188,
	/// <summary>Represents <c>^=</c> token.</summary>
	CaretEqualsToken = 189,
	/// <summary>Represents <c>&lt;&lt;</c> token.</summary>
	LeftShiftToken = 190,
	/// <summary>Represents <c>&gt;&gt;</c> token.</summary>
	RightShiftToken = 191,
	/// <summary>Represents <c>&gt;&gt;&gt;</c> token.</summary>
	UnsignedRightShiftToken = 192,
	/// <summary>Represents <c>&lt;&lt;=</c> token.</summary>
	LeftShiftEqualsToken = 193,
	/// <summary>Represents <c>&gt;&gt;=</c> token.</summary>
	RightShiftEqualsToken = 194,
	/// <summary>Represents <c>&gt;&gt;&gt;=</c> token.</summary>
	UnsignedRightShiftEqualsToken = 195,
	/// <summary>Represents <c>==</c> token.</summary>
	EqualsEqualsToken = 196,
	/// <summary>Represents <c>??</c> token.</summary>
	QuestionQuestionToken = 197,
	/// <summary>Represents <c>??=</c> token.</summary>
	QuestionQuestionEqualsToken = 198,
	/// <summary>Represents <c>-&gt;</c> token.</summary>
	RetusaToken = 199,

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
	ExpressionStatement = 2003,

	// === EXPRESSIONS ===
	ParenthesizedExpression = 3001,
	AddExpression = 3002,
	SubtractExpression = 3003,
	MultiplyExpression = 3004,
	DivideExpression = 3005,
	ModuloExpression = 3006,
	LeftShiftExpression = 3007,
	RightShiftExpression = 3008,
	UnsignedRightShiftExpression = 3009,
	LogicalOrExpression = 3010,
	LogicalAndExpression = 3011,
	BitwiseOrExpression = 3012,
	BitwiseAndExpression = 3013,
	XorExpression = 3014,
	EqualsExpression = 3015,
	NotEqualsExpression = 3016,
	LessThanExpression = 3017,
	LessThanOrEqualExpression = 3018,
	GreaterThanExpression = 3019,
	GreaterThanOrEqualExpression = 3020,
	IsExpression = 3021,
	AsExpression = 3022,
	CoalesceExpression = 3023,
	UnaryAddExpression = 3024,
	UnarySubtractExpression = 3025,
	UnaryLogicalNotExpression = 3026,
	UnaryBitwiseNotExpression = 3027,
	UnaryPointerIndirectionExpression = 3028,
	UnaryAddressOfExpression = 3029,
	AssignmentExpression = 3030,
	AddAssignmentExpression = 3031,
	SubtractAssignmentExpression = 3032,
	MultiplyAssignmentExpression = 3033,
	DivideAssignmentExpression = 3034,
	ModuloAssignmentExpression = 3035,
	AndAssignmentExpression = 3036,
	XorAssignmentExpression = 3037,
	OrAssignmentExpression = 3038,
	LeftShiftAssignmentExpression = 3039,
	RightShiftAssignmentExpression = 3040,
	UnsignedRightShiftAssignmentExpression = 3041,
	CoalesceAssignmentExpression = 3042,

	SimpleName = 3101, // id
	ModuleName = 3102, // id::id::id
	ComplexName = 3103, // Member.Member2
	NameWithExplicitModule = 3104, // id::id::Member || id::id::Member.Member2
	PredefinedType = 3105,
	SliceType = 3106,
	ArrayType = 3107,
	PointerType = 3108,
	ReferenceType = 3109,
	BoxedReferenceType = 3110,
	// TupleType = 3111,

	NumericLiteralExpression = 3201,
	FalseLiteralExpression = 3202,
	TrueLiteralExpression = 3203,

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
	Retusa = 6003,
}