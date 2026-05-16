using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using NiteCompiler.CodeAnalysis.Binding.Operators;

namespace NiteCompiler.CodeAnalysis.Syntax;

// TODO[low]: Generate this file by script
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
	public bool IsBinary => (_value & BinaryFlag) == BinaryFlag;

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

	internal UnaryOperatorKind UnaryOperator
	{
		get
		{
			if (Equals(UnaryAddExpression)) return UnaryOperatorKind.Plus;
			if (Equals(UnarySubtractExpression)) return UnaryOperatorKind.Negate;
			if (Equals(UnaryLogicalNotExpression)) return UnaryOperatorKind.LogicalNot;
			if (Equals(UnaryTildeExpression)) return UnaryOperatorKind.BitwiseNot;
			if (Equals(UnaryCircumflexExpression)) return UnaryOperatorKind.Circumflex;

			throw new UnreachableException();
		}
	}

	internal BinaryOperatorKind BinaryOperator
	{
		get
		{
			if (Equals(AddExpression) || Equals(AddAssignmentExpression))
			{
				return BinaryOperatorKind.Addition;
			}
			if (Equals(SubtractExpression) || Equals(SubtractAssignmentExpression))
			{
				return BinaryOperatorKind.Subtraction;
			}
			if (Equals(MultiplyExpression) || Equals(MultiplyAssignmentExpression))
			{
				return BinaryOperatorKind.Multiplication;
			}
			if (Equals(DivideExpression) || Equals(DivideAssignmentExpression))
			{
				return BinaryOperatorKind.Division;
			}
			if (Equals(ModuloExpression) || Equals(ModuloAssignmentExpression))
			{
				return BinaryOperatorKind.Modulo;
			}
			if (Equals(LeftArithmeticShiftExpression) || Equals(LeftArithmeticShiftAssignmentExpression))
			{
				return BinaryOperatorKind.LeftArithmeticShift;
			}
			if (Equals(RightArithmeticShiftExpression) || Equals(RightArithmeticShiftAssignmentExpression))
			{
				return BinaryOperatorKind.RightArithmeticShift;
			}
			if (Equals(RightUnsignedShiftExpression) || Equals(RightUnsignedShiftAssignmentExpression))
			{
				return BinaryOperatorKind.RightUnsignedShift;
			}
			if (Equals(EqualsExpression))
			{
				return BinaryOperatorKind.Equal;
			}
			if (Equals(NotEqualsExpression))
			{
				return BinaryOperatorKind.NotEqual;
			}
			if (Equals(GreaterExpression))
			{
				return BinaryOperatorKind.Greater;
			}
			if (Equals(LessExpression))
			{
				return BinaryOperatorKind.Less;
			}
			if (Equals(GreaterOrEqualsExpression))
			{
				return BinaryOperatorKind.GreaterOrEqual;
			}
			if (Equals(LessOrEqualsExpression))
			{
				return BinaryOperatorKind.LessOrEqual;
			}
			if (Equals(BitwiseAndExpression) || Equals(BitwiseAndAssignmentExpression))
			{
				return BinaryOperatorKind.And;
			}
			if (Equals(BitwiseOrExpression) || Equals(BitwiseOrAssignmentExpression))
			{
				return BinaryOperatorKind.Or;
			}
			if (Equals(BitwiseXorExpression) || Equals(BitwiseXorAssignmentExpression))
			{
				return BinaryOperatorKind.Xor;
			}
			if (Equals(TildeExpression) || Equals(TildeAssignmentExpression))
			{
				return BinaryOperatorKind.Tilde;
			}

			throw new UnreachableException();
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
			Debug.WriteLine("NokenKind: " + value + " (" + name + ") is already registered.");
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
	public static readonly NodeKind CompilationUnit = Reg(2, "compilation-unit");
	public static readonly NodeKind Token = Reg(3, "<token>");
	public static readonly NodeKind Trivia = Reg(4, "<trivia>");

	private const uint Body = 0x00_00__10_00;
	public static readonly NodeKind EmptyTypeBody = Reg(Body + 1, "empty-type-body");
	public static readonly NodeKind TypeBody = Reg(Body + 2, "type-body");
	public static readonly NodeKind ErrorTypeBody = Reg(Body + 3, "error-type-body");
	public static readonly NodeKind EmptyFunctionBody = Reg(Body + 4, "<empty-function-body>");
	public static readonly NodeKind FunctionBlockBody = Reg(Body + 5, "<function-block-body>");
	public static readonly NodeKind ErrorFunctionBody = Reg(Body + 6, "<error-function-body>");

	private const uint Item = 0x00_00__18_00;
	public static readonly NodeKind ModuleDeclaration = Reg(Item + 1, "module-declaration");
	public static readonly NodeKind FunctionDeclaration = Reg(Item + 2, "function-declaration");
	public static readonly NodeKind TypeDeclaration = Reg(Item + 3, "type-declaration");
	public static readonly NodeKind Parameter = Reg(Item + 4, "parameter");
	public static readonly NodeKind Lifetime = Reg(Item + 5, "lifetime");
	public static readonly NodeKind GenericTypeParameter = Reg(Item + 7, "generic-type-parameter");
	public static readonly NodeKind GenericValueParameter = Reg(Item + 8, "generic-value-parameter");
	public static readonly NodeKind GenericErrorParameter = Reg(Item + 9, "generic-error-parameter");
	public static readonly NodeKind FieldDeclaration = Reg(Item + 10, "field-declaration");
	public static readonly NodeKind ConstructorDeclaration = Reg(Item + 11, "constructor-declaration");
	public static readonly NodeKind NamedConstructorDeclaration = Reg(Item + 12, "named-constructor-declaration");
	public static readonly NodeKind SelfParameter = Reg(Item + 13, "self-parameter");

	private const uint Expression = 0x00_00__20_00;
	private const uint BinaryFlag = 0x00_01__00_00;
	private const uint AssignmentFlag = 0x00_02__00_00;
	public static readonly NodeKind IdentifierNameExpression = Reg(Expression + 1, "identifier-name-expression");
	public static readonly NodeKind GenericNameExpression = Reg(Expression + 3, "generic-name-expression");
	public static readonly NodeKind PathNameExpression = Reg(Expression + 4, "path-name-expression");
	public static readonly NodeKind InlineName = Reg(Expression + 5, "inline-name-expression"); // not used as expression
	public static readonly NodeKind TrueLiteralExpression = Reg(Expression + 7, "true-literal-expression");
	public static readonly NodeKind FalseLiteralExpression = Reg(Expression + 8, "false-literal-expression");
	public static readonly NodeKind ParenthesizedExpression = Reg(Expression + 9, "parenthesized-expression");
	public static readonly NodeKind NumberLiteralExpression = Reg(Expression + 10, "number-literal-expression");
	public static readonly NodeKind CharacterLiteralExpression = Reg(Expression + 11, "character-literal-expression");
	public static readonly NodeKind StringLiteralExpression = Reg(Expression + 12, "string-literal-expression");
	public static readonly NodeKind PredefinedType = Reg(Expression + 13, "predefined-type");
	public static readonly NodeKind PointerType = Reg(Expression + 14, "pointer-type");
	public static readonly NodeKind ReferenceType = Reg(Expression + 15, "reference-type");
	public static readonly NodeKind OptionalType = Reg(Expression + 16, "optional-type");
	public static readonly NodeKind InvocationExpression = Reg(Expression + 17, Precedence.Primary, "invocation-expression");
	public static readonly NodeKind IndexationExpression = Reg(Expression + 18, Precedence.Primary, "indexation-expression");
	public static readonly NodeKind ArrayType = Reg(Expression + 19, "array-type");
	public static readonly NodeKind UnsizedArrayType = Reg(Expression + 20, "unsized-array-type");
	public static readonly NodeKind SelfExpression = Reg(Expression + 21, "self-expression");
	public static readonly NodeKind MemberAccessExpression = Reg(Expression + 22, Precedence.Primary, "member-access-expression");
	private const uint Operation = 0x00_00__28_00;
	public static readonly NodeKind UnaryAddExpression = Reg(Operation + 1, Precedence.Unary, "unary-add-expression");
	public static readonly NodeKind AddExpression = Reg(Operation + BinaryFlag + 2, Precedence.Additive, "add-expression");
	public static readonly NodeKind AddAssignmentExpression = Reg(Operation + BinaryFlag + AssignmentFlag + 3, Precedence.Assignment, "assignment-add-expression");
	public static readonly NodeKind UnarySubtractExpression = Reg(Operation + 4, Precedence.Unary, "unary-subtract-expression");
	public static readonly NodeKind SubtractExpression = Reg(Operation + BinaryFlag + 5, Precedence.Additive, "subtract-expression");
	public static readonly NodeKind SubtractAssignmentExpression = Reg(Operation + BinaryFlag + AssignmentFlag + 6, Precedence.Assignment, "assignment-subtract-expression");
	public static readonly NodeKind MultiplyExpression = Reg(Operation + BinaryFlag + 7, Precedence.Multiplicative, "multiply-expression");
	public static readonly NodeKind MultiplyAssignmentExpression = Reg(Operation + BinaryFlag + AssignmentFlag + 8, Precedence.Assignment, "assignment-multiply-expression");
	public static readonly NodeKind DivideExpression = Reg(Operation + BinaryFlag + 9, Precedence.Multiplicative, "divide-expression");
	public static readonly NodeKind DivideAssignmentExpression = Reg(Operation + BinaryFlag + AssignmentFlag + 10, Precedence.Assignment, "assignment-divide-expression");
	public static readonly NodeKind ModuloExpression = Reg(Operation + BinaryFlag + 11, Precedence.Multiplicative, "modulo-expression");
	public static readonly NodeKind ModuloAssignmentExpression = Reg(Operation + BinaryFlag + AssignmentFlag + 12, Precedence.Assignment, "assignment-module-expression");
	public static readonly NodeKind UnaryTildeExpression = Reg(Operation + 13, Precedence.Unary, "unary-tilde-expression");
	public static readonly NodeKind TildeExpression = Reg(Operation + BinaryFlag + 14, Precedence.Additive, "tilde-expression");
	public static readonly NodeKind TildeAssignmentExpression = Reg(Operation + BinaryFlag + AssignmentFlag + 15, Precedence.Assignment, "assignment-tilde-expression");
	public static readonly NodeKind AddressOfExpression = Reg(Operation + 16, Precedence.Unary, "address-of-expression");
	public static readonly NodeKind BitwiseOrExpression = Reg(Operation + BinaryFlag + 17, Precedence.BitwiseOr, "bitwise-or-expression");
	public static readonly NodeKind BitwiseOrAssignmentExpression = Reg(Operation + BinaryFlag + AssignmentFlag + 18, Precedence.Assignment, "assignment-bitwise-or-expression");
	public static readonly NodeKind UnaryCircumflexExpression = Reg(Operation + 19, Precedence.Unary, "unary-circumflex-expression");
	public static readonly NodeKind BitwiseXorExpression = Reg(Operation + BinaryFlag + 20, Precedence.BitwiseXor, "bitwise-xor-expression");
	public static readonly NodeKind BitwiseXorAssignmentExpression = Reg(Operation + BinaryFlag + AssignmentFlag + 21, Precedence.Assignment, "assignment-bitwise-xor-expression");
	public static readonly NodeKind DereferencingExpression = Reg(Operation + 22, Precedence.Unary, "dereferencing-expression");
	public static readonly NodeKind BitwiseAndExpression = Reg(Operation + BinaryFlag + 23, Precedence.BitwiseAnd, "bitwise-and-expression");
	public static readonly NodeKind BitwiseAndAssignmentExpression = Reg(Operation + BinaryFlag + AssignmentFlag + 24, Precedence.Assignment, "assignment-bitwise-and-expression");
	public static readonly NodeKind ConditionalOrExpression = Reg(Operation + BinaryFlag + 25, Precedence.ConditionalOr, "conditional-or-expression");
	public static readonly NodeKind ConditionalAndExpression = Reg(Operation + BinaryFlag + 26, Precedence.ConditionalAnd, "conditional-and-expression");
	public static readonly NodeKind AssignmentExpression = Reg(Operation + AssignmentFlag + BinaryFlag + 27, Precedence.Assignment, "assignment-expression");
	public static readonly NodeKind GreaterExpression = Reg(Operation + BinaryFlag + 28, Precedence.Relational, "greater-expression");
	public static readonly NodeKind GreaterOrEqualsExpression = Reg(Operation + BinaryFlag + 29, Precedence.Relational, "greater-or-equals-expression");
	public static readonly NodeKind LessExpression = Reg(Operation + BinaryFlag + 30, Precedence.Relational, "less-expression");
	public static readonly NodeKind LessOrEqualsExpression = Reg(Operation + BinaryFlag + 31, Precedence.Relational, "less-or-equals-expression");
	public static readonly NodeKind EqualsExpression = Reg(Operation + BinaryFlag + 32, Precedence.Equality, "equals-expression");
	public static readonly NodeKind NotEqualsExpression = Reg(Operation + BinaryFlag + 33, Precedence.Equality, "not-equals-expression");
	public static readonly NodeKind UnaryLogicalNotExpression = Reg(Operation + 34, Precedence.Unary, "unary-logical-not-expression");
	public static readonly NodeKind LeftArithmeticShiftExpression = Reg(Operation + BinaryFlag + 35, Precedence.Shift, "left-arithmetic-shift-expression");
	public static readonly NodeKind LeftArithmeticShiftAssignmentExpression = Reg(Operation + AssignmentFlag + BinaryFlag + 36, Precedence.Shift, "left-arithmetic-shift-assignment-expression");
	public static readonly NodeKind RightArithmeticShiftExpression = Reg(Operation + BinaryFlag + 37, Precedence.Shift, "right-arithmetic-shift-expression");
	public static readonly NodeKind RightArithmeticShiftAssignmentExpression = Reg(Operation + AssignmentFlag + BinaryFlag + 38, Precedence.Shift, "right-arithmetic-shift-assignment-expression");
	public static readonly NodeKind RightUnsignedShiftExpression = Reg(Operation + BinaryFlag + 39, Precedence.Shift, "right-unsigned-shift-expression");
	public static readonly NodeKind RightUnsignedShiftAssignmentExpression = Reg(Operation + AssignmentFlag + BinaryFlag + 40, Precedence.Shift, "right-unsigned-shift-assignment-expression");
	public static readonly NodeKind RangeExpression = Reg(Operation + BinaryFlag + 41, Precedence.Range, "range-expression");
	public static readonly NodeKind RangeInclusiveExpression = Reg(Operation + BinaryFlag + 42, Precedence.Range, "range-inclusive-expression");

	private const uint Statement = 0x00_00__30_00;
	public static readonly NodeKind ExpressionStatement = Reg(Statement + 1, "expression-statement");
	public static readonly NodeKind EmptyStatement = Reg(Statement + 2, "empty-statement");
	public static readonly NodeKind BlockStatement = Reg(Statement + 3, "block-statement");
	public static readonly NodeKind ReturnStatement = Reg(Statement + 4, "return-statement");
	public static readonly NodeKind IfStatement = Reg(Statement + 5, "if-statement");
	public static readonly NodeKind LoopStatement = Reg(Statement + 6, "loop-statement");
	public static readonly NodeKind WhileStatement = Reg(Statement + 7, "while-statement");
	public static readonly NodeKind ForStatement = Reg(Statement + 8, "for-statement");
	public static readonly NodeKind DoWhileStatement = Reg(Statement + 9, "do-while-statement");
	public static readonly NodeKind ErrorStatement = Reg(Statement + 10, "error-statement");
	public static readonly NodeKind LocalVariableDeclarationStatement = Reg(Statement + 100, "local-variable-declaration-statement");

	private const uint Other = 0x00_00_F0_00;
	public static readonly NodeKind EqualsValueClause = Reg(Other + 1, "equals-value-clause");
	public static readonly NodeKind TypeClause = Reg(Other + 2, "type-clause");
	public static readonly NodeKind ElseClause = Reg(Other + 3, "else-clause");
	public static readonly NodeKind LocalVariableDeclarator = Reg(Other + 4, "local-variable-declarator");
	public static readonly NodeKind ParameterList = Reg(Other + 5, "parameter-list");
	public static readonly NodeKind ArgumentList = Reg(Other + 6, "argument-list");
	public static readonly NodeKind BracketedArgumentList = Reg(Other + 7, "bracketed-argument-list");
	public static readonly NodeKind GenericParameterList = Reg(Other + 8, "generic-parameter-list");
	public static readonly NodeKind GenericArgumentList = Reg(Other + 9, "generic-argument-list");
	public static readonly NodeKind TypeConstraintClause = Reg(Other + 10, "type-constraint-clause");
	public static readonly NodeKind ValueConstraintClause = Reg(Other + 11, "value-constraint-clause");
	public static readonly NodeKind LifetimeConstraintClause = Reg(Other + 12, "lifetime-constraint-clause");
	public static readonly NodeKind ErrorConstraintClause = Reg(Other + 13, "error-constraint-clause");
	public static readonly NodeKind LifetimeConstraint = Reg(Other + 14, "lifetime-constraint");
}