using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NiteCompiler.CodeAnalysis.Syntax;

// ┌───────────────┬───────────────┬───────────────┐
// │ High 8bits    │ Middle 8 bits │ Low 16bits    │
// │ precedence    │ flags         │ operator id   │
// └───────────────┴───────────────┴───────────────┘
// ┌───────────────┬───────────────┐
// │ High 16bits   │ Low 16bits    │
// │ flags         │ raw token id  │
// └───────────────┴───────────────┘
// ┌───────────────┬───────────────┐
// │ High 16bits   │ Low 16bits    │
// │ contextual id │ identifier id │
// └───────────────┴───────────────┘
// ┌───────────────┬───────────────┐
// │ High 16bits   │ Low 16bits    │
// │ zeroes        │ keyword id    │
// └───────────────┴───────────────┘
public readonly struct TokenKind : IEquatable<TokenKind>
{
	// TODO: Use simple custom type or list or array to improve lookup time
	private static readonly Dictionary<ushort, string> _names = [];
	private readonly uint _value;

	public TokenKind()
	{
		_value = 0;
	}

	public ushort HighBits => (ushort)((_value & 0xFFFF0000u) >> 16);
	public ushort RawValue => (ushort)(_value & 0xFFFF);

	public bool IsTrivia => (_value & CategoryFlag) == Trivia;
	public bool IsPunctuator => (_value & CategoryFlag) == Punctuator;
	public bool IsKeyword => (_value & CategoryFlag) == Keyword;
	public bool IsTypeKeyword => (_value & CategoryFlag) == TypeKeyword;
	public PredefinedType AssociatedPredefinedType
	{
		get
		{
			Debug.Assert(IsTypeKeyword);
			return (PredefinedType)(_value - TypeKeyword);
		}
	}
	public bool IsOperator => (_value & CategoryFlag) == Operator;

	public bool IsAssignmentOperator
	{
		get
		{
			Debug.Assert(IsOperator);
			return (_value & AssignmentFlag) == AssignmentFlag;
		}
	}

	public bool CanBeUnaryOperator
	{
		get
		{
			Debug.Assert(IsOperator);
			return (_value & UnaryFlag) == UnaryFlag;
		}
	}
	public bool CanBeBinaryOperator
	{
		get
		{
			Debug.Assert(IsOperator);
			return (_value & BinaryFlag) == BinaryFlag;
		}
	}

	public NodeKind ToLiteralExpressionKind()
	{
		if (this == True) return NodeKind.TrueLiteralExpression;
		if (this == False) return NodeKind.FalseLiteralExpression;
		if (this == NumberLiteral) return NodeKind.NumberLiteralExpression;

		Debug.WriteLine($"ToLiteralExpressionKind({this}) is failed");
		return NodeKind.None;
	}

	public NodeKind ToUnaryExpressionKind()
	{
		Debug.Assert(CanBeUnaryOperator);
		if (this == Plus) return NodeKind.UnaryAddExpression;
		if (this == Minus) return NodeKind.UnarySubtractExpression;
		if (this == Circumflex) return NodeKind.UnaryCircumflexExpression;
		if (this == Asterisk) return NodeKind.DereferencingExpression;
		if (this == Ampersand) return NodeKind.AddressOfExpression;
		if (this == ExclamationSign) return NodeKind.UnaryLogicalNotExpression;
		if (this == Tilde) return NodeKind.UnaryTildeExpression;

		Debug.WriteLine($"ToUnaryExpressionKind({this}) is failed");
		return NodeKind.None;
	}

	public NodeKind ToBinaryExpressionKind()
	{
		Debug.Assert(CanBeBinaryOperator);
		if (this == Plus) return NodeKind.AddExpression;
		if (this == Minus) return NodeKind.SubtractExpression;
		if (this == Asterisk) return NodeKind.MultiplyExpression;
		if (this == Slash) return NodeKind.DivideExpression;
		if (this == Percent) return NodeKind.ModuloExpression;
		if (this == Tilde) return NodeKind.TildeExpression;
		if (this == Pipe) return NodeKind.BitwiseOrExpression;
		if (this == Circumflex) return NodeKind.BitwiseXorExpression;
		if (this == Ampersand) return NodeKind.BitwiseAndExpression;
		if (this == LeftArithmeticShift) return NodeKind.LeftArithmeticShiftExpression;
		if (this == RightArithmeticShift) return NodeKind.RightArithmeticShiftExpression;
		if (this == RightUnsignedShift) return NodeKind.RightUnsignedShiftExpression;

		if (this == DoubleEqual) return NodeKind.EqualsExpression;
		if (this == NotEqual) return NodeKind.NotEqualsExpression;
		if (this == Greater) return NodeKind.GreaterExpression;
		if (this == GreaterOrEquals) return NodeKind.GreaterOrEqualsExpression;
		if (this == Less) return NodeKind.LessExpression;
		if (this == LessOrEquals) return NodeKind.LessOrEqualsExpression;

		if (this == DoubleAmpersand) return NodeKind.ConditionalAndExpression;
		if (this == DoublePipe) return NodeKind.ConditionalOrExpression;

		Debug.WriteLine("ToBinaryExpressionKind is failed");
		return NodeKind.None;
	}

	public NodeKind ToAssignmentExpressionKind()
	{
		Debug.Assert(IsAssignmentOperator);
		if (this == PlusAssignment) return NodeKind.AddAssignmentExpression;
		if (this == MinusAssignment) return NodeKind.SubtractAssignmentExpression;
		if (this == AsteriskAssignment) return NodeKind.MultiplyAssignmentExpression;
		if (this == SlashAssignment) return NodeKind.DivideAssignmentExpression;
		if (this == PercentAssignment) return NodeKind.ModuloAssignmentExpression;
		if (this == TildeAssignment) return NodeKind.TildeAssignmentExpression;
		if (this == PipeAssignment) return NodeKind.BitwiseOrAssignmentExpression;
		if (this == CircumflexAssignment) return NodeKind.BitwiseXorAssignmentExpression;
		if (this == AmpersandAssignment) return NodeKind.BitwiseAndAssignmentExpression;
		if (this == LeftArithmeticShiftAssignment) return NodeKind.LeftArithmeticShiftAssignmentExpression;
		if (this == RightArithmeticShiftAssignment) return NodeKind.RightArithmeticShiftAssignmentExpression;
		if (this == RightUnsignedShiftAssignment) return NodeKind.RightUnsignedShiftAssignmentExpression;

		if (this == Equal) return NodeKind.AssignmentExpression;

		Debug.WriteLine($"ToAssignmentExpressionKind({this}) is failed");
		return NodeKind.None;
	}

	public override string ToString()
	{
		_names.TryGetValue(RawValue, out string? name);

		return name ?? _value.ToString();
	}

	public static implicit operator uint(TokenKind kind)
	{
		return kind._value;
	}

	public static implicit operator TokenKind(uint value)
	{
		return Unsafe.BitCast<uint, TokenKind>(value);
	}

	public bool Equals(TokenKind other)
	{
		return RawValue == other.RawValue;
	}

	public override bool Equals(object? obj)
	{
		return obj is TokenKind other && Equals(other);
	}

	public static bool operator ==(TokenKind lhs, TokenKind rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(TokenKind lhs, TokenKind rhs)
	{
		return !lhs.Equals(rhs);
	}

	public override int GetHashCode()
	{
		return RawValue;
	}

	[Conditional("DEBUG")]
	private static void EnsureNotRegistered(uint value)
	{
		if (_names.TryGetValue((ushort)value, out string? name))
		{
			Debug.Write("TokenKind: " + value + " (" + name + ") is already registered.");
		}
	}

	private static TokenKind Reg(uint value)
	{
		EnsureNotRegistered(value);
		_names[(ushort)value] = "<UNNAMED_TOKEN>";
		return value;
	}

	private static TokenKind Reg(uint value, string name)
	{
		EnsureNotRegistered(value);
		_names[(ushort)value] = name;
		return value;
	}

	public static readonly TokenKind None = Reg(0u, "<none>");
	public static readonly TokenKind EndOfFile = Reg(uint.MaxValue, "<end-of-file>");

	private const uint CategoryFlag = 0x00_00__F0_00u;
	public static readonly TokenKind IdentifierOrKeyword = Reg(1, "<identifier>"); // identifier XID_Start XID_Continue*
	public static readonly TokenKind EscapeIdentifier = Reg(2, "<escape-identifier>"); // `...`
	public static readonly TokenKind LifetimeIdentifier = Reg(3, "<lifetime>"); // ' XID_Continue*
	public static readonly TokenKind NumberLiteral = Reg(4, "<number-literal>");
	public static readonly TokenKind CharacterLiteral = Reg(5, "<char-literal>");

	private const uint Trivia = 0x00_00__10_00u;
	public static readonly TokenKind Whitespace = Reg(Trivia + 1, "<whitespace>");
	public static readonly TokenKind SingleLineComment = Reg(Trivia + 2, "<comment>");
	public static readonly TokenKind MultiLineComment = Reg(Trivia + 3, "<multi-line-comment>");
	public static readonly TokenKind Shebang = Reg(Trivia + 4, "<shebang>");
	public static readonly TokenKind LineBreak = Reg(Trivia + 5, "<line-break>");
	public static readonly TokenKind DocsComment = Reg(Trivia + 6, "<docs-comment>");
	public static readonly TokenKind DocsItemReference = Reg(Trivia + 7, "<docs-item-reference>");

	private const uint Punctuator = 0x00_00__20_00u;
	/// <summary>Represents <c>.</c> token.</summary>
	public static readonly TokenKind Dot = Reg(Punctuator + 1, ".");
	/// <summary>Represents <c>,</c> token.</summary>
	public static readonly TokenKind Comma = Reg(Punctuator + 2, ",");
	/// <summary>Represents <c>'</c> token.</summary>
	public static readonly TokenKind Quote = Reg(Punctuator + 3, "'");
	/// <summary>Represents <c>:</c> token.</summary>
	public static readonly TokenKind Colon = Reg(Punctuator + 4, ":");
	/// <summary>Represents <c>;</c> token.</summary>
	public static readonly TokenKind Semicolon = Reg(Punctuator + 5, ";");
	/// <summary>Represents <c>_</c> token.</summary>
	public static readonly TokenKind Underscore = Reg(Punctuator + 6, "_");
	/// <summary>Represents <c>(</c> token.</summary>
	public static readonly TokenKind OpenParen = Reg(Punctuator + 7, "(");
	/// <summary>Represents <c>)</c> token.</summary>
	public static readonly TokenKind CloseParen = Reg(Punctuator + 8, ")");
	/// <summary>Represents <c>{</c> token.</summary>
	public static readonly TokenKind OpenBrace = Reg(Punctuator + 9, "{");
	/// <summary>Represents <c>}</c> token.</summary>
	public static readonly TokenKind CloseBrace = Reg(Punctuator + 10, "}");
	/// <summary>Represents <c>[</c> token.</summary>
	public static readonly TokenKind OpenBracket = Reg(Punctuator + 11, "[");
	/// <summary>Represents <c>]</c> token.</summary>
	public static readonly TokenKind CloseBracket = Reg(Punctuator + 12, "]");
	/// <summary>Represents <c>-></c> token.</summary>
	public static readonly TokenKind Retusa = Reg(Punctuator + 13, "->");
	/// <summary>Represents <c>::</c> token.</summary>
	public static readonly TokenKind DoubleColon = Reg(Punctuator + 14, "::");

	private const uint Keyword = 0x00_00__30_00u;
	private const uint TypeKeyword = 0x00_00__38_00u;
	public static readonly TokenKind True = Reg(Keyword + 1, "true");
	public static readonly TokenKind False = Reg(Keyword + 2, "false");
	public static readonly TokenKind Use = Reg(Keyword + 3, "use");
	public static readonly TokenKind If = Reg(Keyword + 4, "if");
	public static readonly TokenKind Else = Reg(Keyword + 5, "else");
	public static readonly TokenKind Loop = Reg(Keyword + 6, "loop");
	public static readonly TokenKind While = Reg(Keyword + 7, "while");
	public static readonly TokenKind For = Reg(Keyword + 8, "for");
	public static readonly TokenKind Do = Reg(Keyword + 9, "do");
	public static readonly TokenKind Module = Reg(Keyword + 10, "module");
	public static readonly TokenKind Type = Reg(Keyword + 11, "type");
	public static readonly TokenKind Break = Reg(Keyword + 12, "break");
	public static readonly TokenKind Return = Reg(Keyword + 13, "return");
	public static readonly TokenKind Public = Reg(Keyword + 14, "public");
	public static readonly TokenKind Friend = Reg(Keyword + 15, "friend");
	public static readonly TokenKind Protected = Reg(Keyword + 16, "protected");
	public static readonly TokenKind Internal = Reg(Keyword + 17, "internal");
	public static readonly TokenKind Family = Reg(Keyword + 18, "family");
	public static readonly TokenKind Private = Reg(Keyword + 19, "private");
	public static readonly TokenKind Let = Reg(Keyword + 20, "let");
	public static readonly TokenKind Static = Reg(Keyword + 21, "static");
	public static readonly TokenKind Const = Reg(Keyword + 22, "const");
	public static readonly TokenKind Pure = Reg(Keyword + 23, "pure");

	// Type keywords have they very own unique values
	public static readonly TokenKind I8 = Reg(TypeKeyword + (uint)PredefinedType.I8, "i8");
	public static readonly TokenKind I16 = Reg(TypeKeyword + (uint)PredefinedType.I16, "i16");
	public static readonly TokenKind I32 = Reg(TypeKeyword + (uint)PredefinedType.I32, "i32");
	public static readonly TokenKind I64 = Reg(TypeKeyword + (uint)PredefinedType.I64, "i64");

	public static readonly TokenKind U8 = Reg(TypeKeyword + (uint)PredefinedType.U8, "u8");
	public static readonly TokenKind U16 = Reg(TypeKeyword + (uint)PredefinedType.U16, "u16");
	public static readonly TokenKind U32 = Reg(TypeKeyword + (uint)PredefinedType.U32, "u32");
	public static readonly TokenKind U64 = Reg(TypeKeyword + (uint)PredefinedType.U64, "u64");

	public static readonly TokenKind F16 = Reg(TypeKeyword + (uint)PredefinedType.F16, "f16");
	public static readonly TokenKind F32 = Reg(TypeKeyword + (uint)PredefinedType.F32, "f32");
	public static readonly TokenKind F64 = Reg(TypeKeyword + (uint)PredefinedType.F64, "f64");
	public static readonly TokenKind Void = Reg(TypeKeyword + (uint)PredefinedType.Void, "void");
	public static readonly TokenKind Never = Reg(TypeKeyword + (uint)PredefinedType.Never, "!");

	private const uint Operator = 0x00_00__40_00u;
	private const uint UnaryFlag = 0x00_01__00_00u;
	private const uint BinaryFlag = 0x00_02__00_00u;
	private const uint AssignmentFlag = 0x00_04__00_00u;
	public static readonly TokenKind Plus = Reg(UnaryFlag | BinaryFlag | Operator | 1, "+");
	public static readonly TokenKind PlusAssignment = Reg(BinaryFlag | AssignmentFlag | Operator | 2, "+=");
	public static readonly TokenKind Minus = Reg(UnaryFlag | BinaryFlag | Operator | 3, "-");
	public static readonly TokenKind MinusAssignment = Reg(BinaryFlag | AssignmentFlag | Operator | 4, "-=");
	public static readonly TokenKind Asterisk = Reg(UnaryFlag | BinaryFlag | Operator | 5, "*");
	public static readonly TokenKind AsteriskAssignment = Reg(BinaryFlag | AssignmentFlag | Operator | 6, "*=");
	public static readonly TokenKind Slash = Reg(UnaryFlag | BinaryFlag | Operator | 7, "/");
	public static readonly TokenKind SlashAssignment = Reg(BinaryFlag | AssignmentFlag | Operator | 8, "/=");
	public static readonly TokenKind Percent = Reg(BinaryFlag | Operator | 9, "%");
	public static readonly TokenKind PercentAssignment = Reg(BinaryFlag | AssignmentFlag | Operator | 10, "%=");
	public static readonly TokenKind Tilde = Reg(UnaryFlag | BinaryFlag | Operator | 11, "~");
	public static readonly TokenKind TildeAssignment = Reg(BinaryFlag | AssignmentFlag | Operator | 12, "~=");
	public static readonly TokenKind Pipe = Reg(UnaryFlag | BinaryFlag | Operator | 13, "|");
	public static readonly TokenKind PipeAssignment = Reg(BinaryFlag | AssignmentFlag | Operator | 14, "|=");
	public static readonly TokenKind Circumflex = Reg(UnaryFlag | BinaryFlag | Operator | 15, "^");
	public static readonly TokenKind CircumflexAssignment = Reg(BinaryFlag | AssignmentFlag | Operator | 16, "^=");
	public static readonly TokenKind Ampersand = Reg(UnaryFlag | BinaryFlag | Operator | 17, "&");
	public static readonly TokenKind AmpersandAssignment = Reg(BinaryFlag | AssignmentFlag | Operator | 18, "&=");
	public static readonly TokenKind ExclamationSign = Reg(UnaryFlag | Operator | 19, "!");
	public static readonly TokenKind DoublePipe = Reg(BinaryFlag | Operator | 20, "||");
	public static readonly TokenKind DoubleAmpersand = Reg(BinaryFlag | Operator | 21, "&&");
	public static readonly TokenKind Equal = Reg(BinaryFlag | AssignmentFlag | Operator | 22, "=");
	public static readonly TokenKind DoubleEqual = Reg(BinaryFlag | Operator | 23, "==");
	public static readonly TokenKind NotEqual = Reg(BinaryFlag | Operator | 24, "!=");
	public static readonly TokenKind Greater = Reg(BinaryFlag | Operator | 25, ">");
	public static readonly TokenKind GreaterOrEquals = Reg(BinaryFlag | Operator | 26, ">=");
	public static readonly TokenKind Less = Reg(BinaryFlag | Operator | 27, "<");
	public static readonly TokenKind LessOrEquals = Reg(BinaryFlag | Operator | 28, "<=");
	public static readonly TokenKind LeftArithmeticShift = Reg(BinaryFlag | Operator | 29, "<<");
	public static readonly TokenKind LeftArithmeticShiftAssignment = Reg(BinaryFlag | AssignmentFlag | Operator | 30, "<<=");
	public static readonly TokenKind RightArithmeticShift = Reg(BinaryFlag | Operator | 31, ">>");
	public static readonly TokenKind RightArithmeticShiftAssignment = Reg(BinaryFlag | AssignmentFlag | Operator | 32, ">>=");
	public static readonly TokenKind RightUnsignedShift = Reg(BinaryFlag | Operator | 33, ">>>");
	public static readonly TokenKind RightUnsignedShiftAssignment = Reg(BinaryFlag | AssignmentFlag | Operator | 34, ">>>=");
	public static readonly TokenKind Range = Reg(BinaryFlag | Operator | 35, "..");
	public static readonly TokenKind RangeInclusive = Reg(BinaryFlag | Operator | 36, "..=");
	// #, $, etc. as possible overridable operators?
}