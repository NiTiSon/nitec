using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NiteCompiler.CodeAnalysis.Syntax;

public readonly struct NodeKind : IEquatable<NodeKind>
{
	private static readonly Dictionary<ushort, string> _names = [];
	private readonly uint _value;

	public NodeKind()
	{
		_value = 0;
	}

	public ushort HighBits => (ushort)((_value & 0xFFFF0000u) >> 16);
	public ushort RawValue => (ushort)(_value & 0xFFFF);
	public bool IsExpression => (_value & CategoryFlag) == Expression;

	public bool IsAssignmentExpression => (_value & AssignmentFlag) == AssignmentFlag;

	public bool IsRightAssociative => IsAssignmentExpression;

	private const int PrecedenceShift = 24;
	private const uint PrecedenceMask = 0xFF_00_00_00;
	public Precedence Precedence
	{
		get
		{
			Debug.Assert(IsExpression);
			return (Precedence)((_value & PrecedenceMask) >> PrecedenceShift);
		}
	}

	public override string ToString()
	{
		_names.TryGetValue(RawValue, out string? name);

		return name ?? _value.ToString();
	}

	public static implicit operator uint(NodeKind kind)
	{
		return kind._value;
	}

	public static implicit operator NodeKind(uint value)
	{
		return Unsafe.BitCast<uint, NodeKind>(value);
	}

	public bool Equals(NodeKind other)
	{
		return RawValue == other.RawValue;
	}

	public override bool Equals(object? obj)
	{
		return obj is NodeKind other && Equals(other);
	}

	public static bool operator ==(NodeKind lhs, NodeKind rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(NodeKind lhs, NodeKind rhs)
	{
		return !lhs.Equals(rhs);
	}

	public override int GetHashCode()
	{
		return RawValue;
	}

	[Conditional("DEBUG")]
	private static void EnsureNotRegistered(ushort value)
	{
		if (_names.TryGetValue(value, out string? name))
		{
			Debug.Write("TokenKind: " + value + " (" + name + ") is already registered.");
		}
	}

	private static NodeKind Reg(uint value)
	{
		EnsureNotRegistered((ushort)value);
		_names[(ushort)value] = "<UNNAMED_TOKEN>";
		return value;
	}

	private static NodeKind Reg(uint value, string name)
	{
		EnsureNotRegistered((ushort)value);
		_names[(ushort)value] = name;
		return value;
	}

	private static NodeKind Reg(uint value, Precedence precedence, string name)
	{
		EnsureNotRegistered((ushort)value);
		value += (uint)precedence << PrecedenceShift;
		_names[(ushort)value] = name;
		return value;
	}

	private const uint CategoryFlag = 0x00_00__F0_00u;
	public static readonly NodeKind None = Reg(0, "<none>");
	public static readonly NodeKind SyntaxList = Reg(1, "<syntax-list>");
	public static readonly NodeKind CompilationUnit = Reg(2, "<compilation-unit>");
	public static readonly NodeKind Token = Reg(3, "<token>");
	public static readonly NodeKind Trivia = Reg(4, "<trivia>");

	private const uint Body = 0x00_00__10_00;
	public static readonly NodeKind EmptyTypeBody = Reg(Body + 1, "<empty-type-body>");
	public static readonly NodeKind TypeBody = Reg(Body + 2, "<type-body>");
	public static readonly NodeKind ErrorTypeBody = Reg(Body + 3, "<error-type-body>");

	private const uint Expression = 0x00_00__20_00;
	private const uint BinaryFlag = 0x00_01__00_00;
	private const uint AssignmentFlag = 0x00_02__00_00;
	public static readonly NodeKind SimpleNameExpression = Reg(Expression + 1, "<simple-name-expression>");
	public static readonly NodeKind TrueLiteralExpression = Reg(Expression + 2, "<true-literal-expression>");
	public static readonly NodeKind FalseLiteralExpression = Reg(Expression + 3, "<false-literal-expression>");
	public static readonly NodeKind ParenthesizedExpression = Reg(Expression + 4, "<parenthesized-expression>");
	public static readonly NodeKind NumberLiteralExpression = Reg(Expression + 5, "<number-literal-expression>");
	private const uint Operator = 0x00_00__28_00;
	public static readonly NodeKind UnaryAddExpression = Reg(Operator + BinaryFlag + 1, Precedence.Unary, "unary-add-expression");
	public static readonly NodeKind AddExpression = Reg(Operator + BinaryFlag + 2, Precedence.Additive, "add-expression");
	public static readonly NodeKind AddAssignmentExpression = Reg(Operator + BinaryFlag + AssignmentFlag + 3, Precedence.Assignment, "assignment-add-expression");
	public static readonly NodeKind UnarySubtractExpression = Reg(Operator + BinaryFlag + 4, Precedence.Unary, "unary-subtract-expression");
	public static readonly NodeKind SubtractExpression = Reg(Operator + BinaryFlag + 5, Precedence.Additive, "subtract-expression");
	public static readonly NodeKind SubtractAssignmentExpression = Reg(Operator + BinaryFlag + AssignmentFlag + 6, Precedence.Assignment, "assignment-subtract-expression");
	public static readonly NodeKind MultiplyExpression = Reg(Operator + BinaryFlag + 7, Precedence.Multiplicative, "multiply-expression");
	public static readonly NodeKind MultiplyAssignmentExpression = Reg(Operator + BinaryFlag + AssignmentFlag + 8, Precedence.Assignment, "assignment-multiply-expression");
	public static readonly NodeKind DivideExpression = Reg(Operator + BinaryFlag + 9, Precedence.Multiplicative, "divide-expression");
	public static readonly NodeKind DivideAssignmentExpression = Reg(Operator + BinaryFlag + AssignmentFlag + 10, Precedence.Assignment, "assignment-divide-expression");
	public static readonly NodeKind ModuloExpression = Reg(Operator + BinaryFlag + 11, Precedence.Multiplicative, "modulo-expression");
	public static readonly NodeKind ModuloAssignmentExpression = Reg(Operator + BinaryFlag + AssignmentFlag + 12, Precedence.Assignment, "assignment-module-expression");
	public static readonly NodeKind UnaryTildaExpression = Reg(Operator + 13, Precedence.Unary, "unary-tilda-expression");
	public static readonly NodeKind TildaExpression = Reg(Operator + BinaryFlag + 14, Precedence.Additive, "tilda-expression");
	public static readonly NodeKind TildaAssignmentExpression = Reg(Operator + BinaryFlag + AssignmentFlag + 15, Precedence.Assignment, "assignment-tilda-expression");
	public static readonly NodeKind AddressOfExpression = Reg(Operator + 16, Precedence.Unary, "address-of-expression");
	public static readonly NodeKind BitwiseOrExpression = Reg(Operator + BinaryFlag + 17, Precedence.BitwiseOr, "bitwise-or-expression");
	public static readonly NodeKind BitwiseOrAssignmentExpression = Reg(Operator + BinaryFlag + AssignmentFlag + 18, Precedence.Assignment, "assignment-bitwise-or-expression");
	public static readonly NodeKind UnaryCircumflexExpression = Reg(Operator + 19, Precedence.Unary, "unary-circumflex-expression");
	public static readonly NodeKind BitwiseXorExpression = Reg(Operator + BinaryFlag + 20, Precedence.BitwiseXor, "bitwise-xor-expression");
	public static readonly NodeKind BitwiseXorAssignmentExpression = Reg(Operator + BinaryFlag + AssignmentFlag + 21, Precedence.Assignment, "assignment-bitwise-xor-expression");
	public static readonly NodeKind DereferencingExpression = Reg(Operator + 22, Precedence.Unary, "dereferencing-expression");
	public static readonly NodeKind BitwiseAndExpression = Reg(Operator + BinaryFlag + 23, Precedence.BitwiseAnd, "bitwise-and-expression");
	public static readonly NodeKind BitwiseAndAssignmentExpression = Reg(Operator + BinaryFlag + AssignmentFlag + 24, Precedence.Assignment, "assignment-bitwise-and-expression");
	public static readonly NodeKind ConditionalOrExpression = Reg(Operator + BinaryFlag + 25, Precedence.ConditionalOr, "conditional-or-expression");
	public static readonly NodeKind ConditionalAndExpression = Reg(Operator + BinaryFlag + 26, Precedence.ConditionalAnd, "conditional-and-expression");
	public static readonly NodeKind AssignmentExpression = Reg(Operator + AssignmentFlag + BinaryFlag + 27, Precedence.Assignment, "assignment-expression");
}