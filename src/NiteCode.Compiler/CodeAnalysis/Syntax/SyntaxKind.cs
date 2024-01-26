using System;
using static NiteCode.Compiler.CodeAnalysis.Syntax.SyntaxKindFlags;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public enum SyntaxKind
{
	None = 0,

	// Punctuation tokens

	/// <summary>
	/// Represents <c>.</c> token.
	/// </summary>
	DotToken = TokenFlag + 1,
	/// <summary>
	/// Represents <c>..</c> token.
	/// </summary>
	DotDotToken,
	/// <summary>
	/// Represents <c>!</c> token.
	/// </summary>
	ExclamationToken,
	/// <summary>
	/// Represents <c>?</c> token.
	/// </summary>
	QuestionToken,
	/// <summary>
	/// Represents <c>%</c> token.
	/// </summary>
	PercentToken,
	/// <summary>
	/// Represents <c>~</c> token.
	/// </summary>
	TildeToken,
	/// <summary>
	/// Represents <c>&amp;</c> token.
	/// </summary>
	AmpersandToken,
	/// <summary>
	/// Represents <c>^</c> token.
	/// </summary>
	CircumflexToken,
	/// <summary>
	/// Represents <c>(</c> token.
	/// </summary>
	OpenParenToken,
	/// <summary>
	/// Represents <c>)</c> token.
	/// </summary>
	CloseParenToken,
	/// <summary>
	/// Represents <c>[</c> token.
	/// </summary>
	OpenBracketToken,
	/// <summary>
	/// Represents <c>]</c> token.
	/// </summary>
	CloseBracketToken,
	/// <summary>
	/// Represents <c>{</c> token.
	/// </summary>
	OpenBraceToken,
	/// <summary>
	/// Represents <c>}</c> token.
	/// </summary>
	CloseBraceToken,
	/// <summary>
	/// Represents <c>+</c> token.
	/// </summary>
	PlusToken,
	/// <summary>
	/// Represents <c>-</c> token.
	/// </summary>
	MinusToken,
	/// <summary>
	/// Represents <c>*</c> token.
	/// </summary>
	AsteriskToken,
	/// <summary>
	/// Represents <c>/</c> token.
	/// </summary>
	SlashToken,
	/// <summary>
	/// Represents <c>\</c> token.
	/// </summary>
	BackslashToken,
	/// <summary>
	/// Represents <c>=</c> token.
	/// </summary>
	EqualsToken,
	/// <summary>
	/// Represents <c>|</c> token.
	/// </summary>
	BarToken,
	/// <summary>
	/// Represents <c>:</c> token.
	/// </summary>
	ColonToken,
	/// <summary>
	/// Represents <c>;</c> token.
	/// </summary>
	SemicolonToken,

	// Compound punctuation tokens

	/// <summary>
	/// Represents <c>||</c> token.
	/// </summary>
	BarBarToken,
	/// <summary>
	/// Represents <c>&amp;&amp;</c> token.
	/// </summary>
	AmpersandAmpersandToken,
	/// <summary>
	/// Represents <c>--</c> token.
	/// </summary>
	MinusMinusToken,
	/// <summary>
	/// Represents <c>++</c> token.
	/// </summary>
	PlusPlusToken,
	/// <summary>
	/// Represents <c>::</c> token.
	/// </summary>
	ColonColonToken,
	/// <summary>
	/// Represents <c>!=</c> token.
	/// </summary>
	ExclamationEqualsToken,
	/// <summary>
	/// Represents <c>==</c> token.
	/// </summary>
	EqualsEqualsToken,

	// Keywords
	/// <summary>
	/// Represents <see langword="bool"/>.
	/// </summary>
	BoolKeyword = KeywordFlag + 1,
	/// <summary>
	/// Represents <see langword="i8"/>.
	/// </summary>
	I8Keyword,
	/// <summary>
	/// Represents <see langword="i16"/>.
	/// </summary>
	I16Keyword,
	/// <summary>
	/// Represents <see langword="i32"/>.
	/// </summary>
	I32Keyword,
	/// <summary>
	/// Represents <see langword="i64"/>.
	/// </summary>
	I64Keyword,
	/// <summary>
	/// Represents <see langword="u8"/>.
	/// </summary>
	U8Keyword,
	/// <summary>
	/// Represents <see langword="u16"/>.
	/// </summary>
	U16Keyword,
	/// <summary>
	/// Represents <see langword="u32"/>.
	/// </summary>
	U32Keyword,
	/// <summary>
	/// Represents <see langword="u64"/>.
	/// </summary>
	U64Keyword,
	/// <summary>
	/// Represents <see langword="f16"/>.
	/// </summary>
	F16Keyword,
	/// <summary>
	/// Represents <see langword="f32"/>.
	/// </summary>
	F32Keyword,
	/// <summary>
	/// Represents <see langword="f64"/>.
	/// </summary>
	F64Keyword,
	/// <summary>
	/// Represents <see langword="void"/>.
	/// </summary>
	VoidKeyword,
	/// <summary>
	/// Represents <see langword="sizeof"/>.
	/// </summary>
	SizeofKeyword,
	/// <summary>
	/// Represents <see langword="typeof"/>.
	/// </summary>
	TypeofKeyword,
	/// <summary>
	/// Represents <see langword="nil"/>.
	/// </summary>
	NilKeyword,
	/// <summary>
	/// Represents <see langword="true"/>.
	/// </summary>
	TrueKeyword,
	/// <summary>
	/// Represents <see langword="false"/>.
	/// </summary>
	FalseKeyword,
	/// <summary>
	/// Represents <see langword="ref"/>.
	/// </summary>
	RefKeyword,
	/// <summary>
	/// Represents <see langword="public"/>.
	/// </summary>
	PublicKeyword,
	/// <summary>
	/// Represents <see langword="private"/>.
	/// </summary>
	PrivateKeyword,
	/// <summary>
	/// Represents <see langword="protected"/>.
	/// </summary>
	ProtectedKeyword,
	/// <summary>
	/// Represents <see langword="internal"/>.
	/// </summary>
	InternalKeyword,
	/// <summary>
	/// Represents <see langword="family"/>.
	/// </summary>
	FamilyKeyword,
	/// <summary>
	/// Represents <see langword="friend"/>.
	/// </summary>
	FriendKeyword,
	/// <summary>
	/// Represents <see langword="static"/>.
	/// </summary>
	StaticKeyword,
	/// <summary>
	/// Represents <see langword="virtual"/>.
	/// </summary>
	VirtualKeyword,
	/// <summary>
	/// Represents <see langword="abstract"/>.
	/// </summary>
	AbstractKeyword,
	/// <summary>
	/// Represents <see langword="override"/>.
	/// </summary>
	OverrideKeyword,
	/// <summary>
	/// Represents <see langword="sealed"/>.
	/// </summary>
	SealedKeyword,
	/// <summary>
	/// Represents <see langword="extern"/>.
	/// </summary>
	ExternKeyword,
	/// <summary>
	/// Represents <see langword="operator"/>.
	/// </summary>
	OperatorKeyword,
	/// <summary>
	/// Represents <see langword="new"/>.
	/// </summary>
	NewKeyword,
	/// <summary>
	/// Represents <see langword="delete"/>.
	/// </summary>
	DeleteKeyword,
	/// <summary>
	/// Represents <see langword="self"/>.
	/// </summary>
	SelfKeyword,
	/// <summary>
	/// Represents <see langword="base"/>.
	/// </summary>
	BaseKeyword,
	/// <summary>
	/// Represents <see langword="module"/>.
	/// </summary>
	ModuleKeyword,
	/// <summary>
	/// Represents <see langword="using"/>.
	/// </summary>
	UsingKeyword,
	/// <summary>
	/// Represents <see langword="type"/>.
	/// </summary>
	TypeKeyword,

	// Contextual keywords tokens (current no one...)

	// Other

	EndOfFileToken,

	// Text tokens

	BadToken,
	IdentifierToken,
	NumericLiteralToken,
	CharacterLiteralToken,
	StringLiteralToken,

	// Trivia

	EndOfLineTrivia,
	WhitespaceTrivia,
	SingleLineCommentTrivia,
	MultiLineCommentTrivia,
	SkippedTokensTrivia,
	DisabledTextTrivia,

	// Names and Type names

	/// <summary>
	/// Represents Identifier.
	/// </summary>
	IdentifierName,
	/// <summary>
	/// Represents <c>Identifier.Identifier</c> or <c>Identifier.Identifier<![CDATA[<T>]]></c>.
	/// </summary>
	QualifiedName,
	/// <summary>
	/// Represents Identifier<![CDATA[<T>]]></c>
	/// </summary>
	GenericName,
	/// <summary>
	/// Represents &lt;Type, Literal, ...&gt; 
	/// </summary>
	TypeArgumentList,
	/// <summary>
	/// Represent any predefined type, like <c>u32</c> etc.
	/// </summary>
	PredefinedType,
	/// <summary>
	/// Represents pointer to other type.
	/// </summary>
	PointerType,
	/// <summary>
	/// Represents reference to other type.
	/// </summary>
	ReferenceType,
	/// <summary>
	/// Represents <c>[TYPE]</c>.
	/// </summary>
	ArrayType,
	/// <summary>
	/// Represents <c>[TYPE; SIZE]</c>.
	/// </summary>
	SliceArrayType,
	[Obsolete("Nullable type are not supported yet; otherwise remove this annotation.")]
	/// <remarks>
	/// Not supported yet.
	/// </remarks>
	NullableType,

	// Expressions

	ParenthesizedExpression = ExpressionFlag + 1,
	ElementAccessExpression,
	InvocationExpression,

	// Unary expressions
	BitwiseNotExpression,
	LogicalNotExpression,
	PrefixDecrementExpression,
	PrefixIncrementExpression,
	PostfixDecrementExpression,
	PostfixIncrementExpression,

	// Binary expressions
	
	AddExpression,
	SubstractExpression,
	MultiplyExpression,
	DivideExpression,
	ModuloExpression,
	// TODO: shifts
	// TODO: logical ops
	// TODO: comp ops
	SimpleMemberAccessExpression,
	SimpleMethodAccessExpression,
	ConditionalMemberAccessExpression,
	ConditionalMethodAccessExpression,

	// Binary assignment expressions

	SimpleAssignmentExpression,

	// Primary expressions

	SelfExpression,
	BaseExpression,
	NumericLiteralExpression,
	CharacterLiteralExpression,
	StringLiteralExpression,
	TrueLiteralExpression,
	FalseLiteralExpression,
	NilLiteralExpression,
	DefaultLiteralExpression,

	// Primary function expression

	TypeofExpression,
	TypeofGenericExpression,
	SizeofExpression,
	SizeofGenericExpression,

	// Statements
	
	BlockStatement,
	LocalDeclarationStatement,
	VariableDeclaration,
	VariableDeclarator,
	EqualsValueClause,
	ExpressionStatement,
	EmptyStatement,
	LabeledStatement,

	// Jump statements

	GotoStatement,
	BreakStatement,
	ContinueStatement,
	ReturnStatement,
	ThrowStatement,

	WhileStatement,
	DoStatement,
	ForStatement,
	LoopStatement,

	// Declarations
	CompilationUnit,
	ModuleDeclaration,
	UsingDirective,

	// Type declarations
	TypeDeclaration,

	// TODO: base list
	FieldDeclaration,
	ConstructorDeclaration,
	DestructorDeclaration,
	MethodDeclaration,
	ParameterList,
	BracketedParameterList,
	Parameter,
	GenericParameterList,
	GenericParameter,
	IncompleteMember,
}

internal static class SyntaxKindFlags
{
	public const int TokenFlag = 2048;
	public const int KeywordFlag = 4096;
	public const int ExpressionFlag = 8192;
}