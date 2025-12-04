using System;
using System.Runtime.CompilerServices;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.CodeAnalysis.Text;
using NiteCompiler.Compilation;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed  class ExpressionBinder : Binder
{
	public ExpressionBinder(NiteCompilation niteCompilation, Binder parent) : base(niteCompilation, parent)
	{
	}

	protected override Symbol? LookupSymbol(string name, LookupOptions options = LookupOptions.Default)
	{
		return LookupSymbolInParent(name, options);
	}

	public override BoundNode Bind(SyntaxNode syntax)
	{
		if (syntax is ExpressionSyntax expression)
			return BindExpression(expression);

		throw new ArgumentException(null, nameof(syntax));
	}

	public BoundExpression BindExpression(ExpressionSyntax syntax, bool isNegative = false)
	{
		return syntax switch
		{
			//IdentifierExpressionSyntax id => BindIdentifier(id),
			LiteralExpressionSyntax literal => BindLiteral(literal, isNegative),
			UnaryExpressionSyntax unary => BindUnary(unary),
			BinaryExpressionSyntax binary => BindBinary(binary),
			//CallExpressionSyntax call => BindCall(call),
			_ => throw new InvalidOperationException("Unknown expression syntax")
		};
	}

	private BoundExpression BindLiteral(LiteralExpressionSyntax syntax, bool isNegative = false)
	{
		if (syntax.Token is not NumberToken value) throw new ArgumentException(null, nameof(syntax));

		PredefinedType suitable = GetMinimalSuitableInteger(value.Value.U64, syntax.Location, isNegative);
		suitable = GetSuitableInteger(suitable, syntax.Location, value.Type);

		return new BoundLiteralExpression(syntax, null, value);
	}

	private PredefinedType GetSuitableInteger(PredefinedType minimum, Location location, NumericLiteralType type)
	{
		var min32Bit = UpcastTo32Bit(minimum);
		if (type == NumericLiteralType.None) return min32Bit;

		if (type == NumericLiteralType.Signed)
		{
			if (min32Bit is PredefinedType.I32 or PredefinedType.I64) return minimum; // 000i is only i32 or i64

			NiteCompilation.Diagnostics.ReportIntegralValueCantBeSigned(location);
		}

		if (type == NumericLiteralType.Unsigned)
		{
			return min32Bit switch
			{
				PredefinedType.I32 => PredefinedType.U32,
				PredefinedType.I64 => PredefinedType.U64,
				_ => min32Bit
			};
		}

		if (CanUpcast(minimum, type, out PredefinedType toType))
		{
			return toType;
		}
		else
		{
			return minimum;
		}

		static PredefinedType UpcastTo32Bit(PredefinedType type)
		{
			if (type is PredefinedType.I8 or PredefinedType.I16) return PredefinedType.I32;
			if (type is PredefinedType.U8 or PredefinedType.U16) return PredefinedType.U32;
			return type;
		}
	}

	private static bool CanUpcast(PredefinedType from, NumericLiteralType to, out PredefinedType toType)
	{
		bool fromUnsigned = from is >= PredefinedType.U8 and <= PredefinedType.U64;
		bool toUnsigned = false;
		if (to is >= NumericLiteralType.I8 and <= NumericLiteralType.I64)
		{
			toType = (PredefinedType)(to - NumericLiteralType.I8);
		}
		else
		{
			toUnsigned = true;
			toType = (PredefinedType)(to - NumericLiteralType.U8);
		}

		if (fromUnsigned == toUnsigned)
		{
			return from - toType <= 0;
		}
		if (fromUnsigned && !toUnsigned)
		{
			return from < toType;
		}
		if (!fromUnsigned && toUnsigned)
		{
			return false;
		}

		return false;
	}

	/// <summary>
	/// This type resolution for integral types is a FUCKING SHIT, if you know how to make better, please do 👽👽👽
	/// </summary>
	private PredefinedType GetMinimalSuitableInteger(ulong value, Location location, bool isNegative)
	{
		// Check if fallout of ulong.MinValue
		// God bless One's complement
		if (isNegative && value > Unsafe.BitCast<long, ulong>(long.MinValue))
		{
			NiteCompilation.Diagnostics.ReportIntegralValueIsSmallerThanMinValue(location);

			return PredefinedType.U64;
		}

		if (isNegative)
		{
			if (value <= Unsafe.BitCast<sbyte, byte>(sbyte.MinValue))
			{
				return PredefinedType.I8;
			}
			if (value <= Unsafe.BitCast<short, ushort>(short.MinValue))
			{
				return PredefinedType.I16;
			}
			if (value <= Unsafe.BitCast<int, uint>(int.MinValue))
			{
				return PredefinedType.I32;
			}

			return PredefinedType.I64;
		}
		else
		{
			if (value <= (ulong)sbyte.MaxValue)
			{
				return PredefinedType.I8;
			}
			if (value <= (ulong)short.MaxValue)
			{
				return PredefinedType.I16;
			}
			if (value <= int.MaxValue)
			{
				return PredefinedType.I32;
			}
			if (value <= long.MaxValue)
			{
				return PredefinedType.I64;
			}

			return PredefinedType.U64;
		}
	}

	// private BoundExpression BindIdentifier(IdentifierExpressionSyntax id)
	// {
	// 	var symbol = LookupSymbol(id.Name)
	// 	             ?? throw new Exception($"Unknown identifier {id.Name}");
	//
	//
	// 	return symbol switch
	// 	{
	// 		LocalVariableSymbol local => new BoundLocalVariable(local),
	// 		FieldSymbol field => new BoundFieldAccess(field),
	// 		ParameterSymbol param => new BoundParameter(param),
	// 		TypeSymbol t => throw new Exception("Type used as value"),
	// 		_ => throw new Exception("Invalid identifier symbol type.")
	// 	};
	// }

	private BoundExpression BindUnary(UnaryExpressionSyntax syntax)
	{
		bool isNegative = syntax.Kind == SyntaxKind.UnarySubtractExpression;

		BoundExpression value = BindExpression(syntax.Expression, isNegative);

		return new BoundUnaryExpression(syntax, null, value);
	}

	private BoundExpression BindBinary(BinaryExpressionSyntax syntax)
	{
		BoundExpression left = BindExpression(syntax.Left);
		BoundExpression right = BindExpression(syntax.Right);

		//var op = BoundBinaryOperator.Bind(syntax.OperatorToken, left.Type, right.Type);
		//if (op == null)
		//	throw new Exception($"Operator {syntax.OperatorToken} not defined for '{left.Type.Name}' and '{right.Type.Name}'");

		return new BoundBinaryExpression(syntax, left, null, right);
	}
}