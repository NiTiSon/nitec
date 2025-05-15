namespace Nlr.Compiler.CodeAnalysis.Syntax;

public enum SyntaxKind : uint
{
	// === System [0..1024] ===

	/// <summary>
	/// Token is determine EOF.
	/// </summary>
	EndOfFile = 0,

	/// <summary>
	/// Token type is unknown or wrong.
	/// </summary>
	UnknownOrWrong = 1,

	// === Trivia [1024..2048] ===

	WhitespaceTrivia = 1024,
	LineBreakTrivia = 1025,
	SingleLineCommentTrivia = 1026,
	MultiLineCommentTrivia = 1027,


	// === Tokens [2048..8192] ===

	/// <summary>
	/// Identifier token.
	/// </summary>
	IdentifierToken = 2048,
	/// <summary>
	/// Number token.
	/// </summary>
	NumberToken = 2049,
	/// <summary>Represents <c>+</c> token.</summary>
	PlusToken = 2050,
	/// <summary>Represents <c>-</c> token.</summary>
	MinusToken = 2051,
	/// <summary>Represents <c>%</c> token.</summary>
	PercentToken = 2052,
	/// <summary>Represents <c>*</c> token.</summary>
	AsteriskToken = 2053,
	/// <summary>Represents <c>/</c> token.</summary>
	SlashToken = 2054,
	/// <summary>Represents <c>:</c> token.</summary>
	ColonToken = 2055,
	/// <summary>Represents <c>;</c> token.</summary>
	SemicolonToken = 2056,
	/// <summary>Represents <c>?</c> token.</summary>
	QuestionMarkToken = 2057,
	/// <summary>Represents <c>!</c> token.</summary>
	ExclamationMarkToken = 2058,
	/// <summary>Represents <c>&amp;</c> token.</summary>
	AmpersandToken = 2059,
	/// <summary>Represents <c>^</c> token.</summary>
	CaretToken = 2060,
	/// <summary>Represents <c>|</c> token.</summary>
	PipeToken = 2061,
	/// <summary>Represents <c>=</c> token.</summary>
	EqualsToken = 2062,
	/// <summary>Represents <c>\</c> token.</summary>
	BackslashToken = 2063,
	/// <summary>Represents <c>'</c> token.</summary>
	SingleQuoteToken = 2064,
	/// <summary>Represents <c>"</c> token.</summary>
	DoubleQuoteToken = 2065,
	/// <summary>Represents <c>.</c> token.</summary>
	DotToken = 2066,
	/// <summary>Represents <c>&lt;</c> token.</summary>
	LessThanToken = 2067,
	/// <summary>Represents <c>&gt;</c> token.</summary>
	GreaterThanToken = 2068,
	/// <summary>Represents <c>#</c> token.</summary>
	HashToken = 2069,
	/// <summary>Represents <c>[</c> token.</summary>
	OpenBracketToken = 2070,
	/// <summary>Represents <c>]</c> token.</summary>
	CloseBracketToken = 2071,
	/// <summary>Represents <c>{</c> token.</summary>
	OpenBraceToken = 2072,
	/// <summary>Represents <c>}</c> token.</summary>
	CloseBraceToken = 2073,
	/// <summary>Represents <c>(</c> token.</summary>
	OpenParenToken = 2074,
	/// <summary>Represents <c>)</c> token.</summary>
	CloseParenToken = 2075,
	/// <summary>Represents <c>~</c> token.</summary>
	TildaToken = 2076,
	/// <summary>Represents <c>--</c> token.</summary>
	MinusMinusToken = 2077,
	/// <summary>Represents <c>++</c> token.</summary>
	PlusPlusToken = 2078,
	/// <summary>Represents <c>&lt;&lt;</c> token.</summary>
	LessThanLessThanToken = 2079,
	/// <summary>Represents <c>&gt;&gt;</c> token.</summary>
	GreaterThanGreaterThanToken = 2080,
	/// <summary>Represents <c>==</c> token.</summary>
	EqualsEqualsToken = 2081,
	/// <summary>Represents <c>!=</c> token.</summary>
	ExclamationMarkEqualsToken = 2082,
	/// <summary>Represents <c>-&gt;</c> token.</summary>
	MinusGreaterThanToken = 2083,
	/// <summary>Represents <c>||</c> token.</summary>
	PipePipeToken = 2084,
	/// <summary>Represents <c>::</c> token.</summary>
	ColonColonToken = 2085,
	/// <summary>Represents <c>&&</c> token.</summary>
	AmpersandAmpersandToken = 2086,
	/// <summary>Represents <c>?.</c> token.</summary>
	QuestionMarkDotToken = 2087,
	/// <summary>Represents <c>!.</c> token.</summary>
	ExclamationMarkDotToken = 2088,
	/// <summary>Represents <c>??</c> token.</summary>
	QuestionMarkQuestionMarkToken = 2089,
	/// <summary>Represents <c>~=</c> token.</summary>
	TildaEqualsToken = 2090,
	/// <summary>Represents <c>|=</c> token.</summary>
	PipeEqualsToken = 2091,
	/// <summary>Represents <c>&amp;=</c> token.</summary>
	AmpersandEqualsToken = 2092,
	/// <summary>Represents <c>%=</c> token.</summary>
	PercentEqualsToken = 2093,
	/// <summary>Represents <c>/=</c> token.</summary>
	SlashEqualsToken = 2094,
	/// <summary>Represents <c>*=</c> token.</summary>
	AsteriskEqualsToken = 2095,
	/// <summary>Represents <c>-=</c> token.</summary>
	MinusEqualsToken = 2096,
	/// <summary>Represents <c>+=</c> token.</summary>
	PlusEqualsToken = 2097,
	/// <summary>Represents <c>&gt;=</c> token.</summary>
	GreaterThanEqualsToken = 2098,
	/// <summary>Represents <c>&lt;=</c> token.</summary>
	LessThanEqualsToken = 2099,
	/// <summary>Represents <c>..</c> token.</summary>
	DotDotToken = 2100,
	/// <summary>Represents <c>..=</c> token.</summary>
	DotDotEqualsToken = 2101,
	
	/// <summary>Represents <c>??=</c> token.</summary>
	QuestionMarkQuestionMarkEqualsToken = 2102,
	/// <summary>Represents <c>&lt;&lt;=</c> token.</summary>
	LessThanLessThanEqualsToken = 2103,
	/// <summary>Represents <c>&gt;&gt;&gt;</c> token.</summary>
	GreaterThanGreaterThanGreaterThanToken = 2104,
	/// <summary>Represents <c>&gt;&gt;&gt;=</c> token.</summary>
	GreaterThanGreaterThanGreaterThanEqualsToken = 2105,
	/// <summary>Represents <c>&gt;&gt;=</c> token.</summary>
	GreaterThanGreaterThanEqualsToken = 2106,

	// === Keywords [8192..32768] ===

	TrueKeyword = 8192,
	FalseKeyword = 8193,
	UseKeyword = 8194,
	IfKeyword = 8195,
	ElseKeyword = 8196,
	LoopKeyword = 8197,
	ForKeyword = 8198,
	DoKeyword = 8199,
	WhileKeyword = 8200,
	TypeofKeyword = 8201,
	SizeofKeyword = 8202,
	OffsetofKeyword = 8203,
	DefaultKeyword = 8204,
	BreakKeyword = 8205,
	ReturnKeyword = 8206,
	ContinueKeyword = 8207,
	WhereKeyword = 8208,
	WhenKeyword = 8209,
	IsKeyword = 8210,
	AsKeyword = 8211,
	ConstKeyword = 8212,
	PublicKeyword = 8213,
	FriendKeyword = 8214,
	ProtectedKeyword = 8215,
	InternalKeyword = 8216,
	FamilyKeyword = 8217,
	PrivateKeyword = 8218,
	VirtualKeyword = 8219,
	StaticKeyword = 8220,
	VolatileKeyword = 8221,
	GetKeyword = 8222,
	SetKeyword = 8223,
	OperatorKeyword = 8224,
	U8Keyword = 8225,
	U16Keyword = 8226,
	U32Keyword = 8227,
	U64Keyword = 8228,
	I8Keyword = 8229,
	I16Keyword = 8230,
	I32Keyword = 8231,
	I64Keyword = 8232,
	F16Keyword = 8233,
	F32Keyword = 8234,
	F64Keyword = 8235,
	VoidKeyword = 8236,
	BoolKeyword = 8237,
	ModuleKeyword = 8238,
	TypeKeyword = 8239,
	
	// === Nodes [32768..65536] ===
	
	CompilationUnit = 32768,
	
	IdentifierName, // identifier
	ModuleName, // x || x::y || x::y::z ...
	FullyQualifiedName,
	PredefinedType,
	
	// Expressions
	ParenthesizedExpression,
	UnaryExpression,
	BinaryExpression,
	NameExpression,
	LiteralExpression,
	
	// Statements
	BlockStatement,
	
	// Branch statements
	ReturnStatement,
	ForStatement,
	ContinueStatement,
	BreakStatement,
	
	// Declarations
	ModuleDeclaration,
	TypeDeclaration,
	FunctionDeclaration,
	FieldDeclaration,
	IncompleteMemberDeclaration,
	
	// Directives
	UseDirective,
	
	// Lists
	ReturnParameterList,
	ParameterList,
}