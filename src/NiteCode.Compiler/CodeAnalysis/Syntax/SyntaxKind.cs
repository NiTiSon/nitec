namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public enum SyntaxKind : uint
{
	BadToken,

	EndOfFile,
	NumberLiteral,
	StringLiteral,
	Identifier,

	SymbolFlag = 0x0C000000, // For special symbols and keywords
	NilConst,
	TrueConst,
	FalseConst,
	
	OperatorFlag = 0x3C000000, // For operators
	Plus,
	PlusPlus,
	PlusEquals,
	Minus,
	MinusMinus,
	MinusEquals,
	Asterisk,
	AsteriskEquals,
	Slash,
	SlashEquals,
	Modulo,
	ModuloEquals,
	
	Bang,
	Equals,
	EqualsEquals,
	BangEquals,
	Tilde,
	Circumflex, // a.k.a. Hat ^_^
	CircumflexEquals,
	Ampersand,
	AmpersandAmpersand,
	AmpersandEquals,
	Bar,
	BarBar,
	BarEquals,

	Less,
	LessLess,
	LessEquals,
	LessLessEquals,
	More,
	MoreMore,
	MoreEquals,
	MoreMoreEquals,

	
	BuiltInTypeFlag = 0x2C000000,
	I8,
	I16,
	I32,
	I64,
	I128,
	U8,
	U16,
	U32,
	U64,
	U128,
	F16,
	F32,
	F64,
	F128,
	Bool,
	Void,

	KeywordFlag = 0x1C000000,
	PublicKeyword,
	PrivateKeyword,
	ProtectedKeyword,
	
	NewKeyword,
	DeleteKeyword,

	TypeKeyword,
	EnumKeyword,
	
	RefKeyword,
	ObjectKeyword,
	LocalKeyword,
	
	ConstKeyword,
	LiteralKeyword,
	StaticKeyword,
	
	SizeofKeyword,
	TypeofKeyword,
	DefaultKeyword,

	OperatorKeyword,
	ExternalKeyword,
	ImplicitKeyword,

	IfKeyword,
	ElseKeyword,
	WhileKeyword,
	DoKeyword,
	ReturnKeyword,

	TriviaFlag = 0x2A000000,
	SkippedTextTrivia,
	LineBreakTrivia,
	WhitespaceTrivia,
	SingleLineCommentTrivia,
	MultiLineCommentTrivia,
}