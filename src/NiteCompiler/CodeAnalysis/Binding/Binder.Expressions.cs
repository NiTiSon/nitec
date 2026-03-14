using System;
using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Binding.BoundTree;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Binding;

internal partial class Binder
{
	private const int ValueKindInsignificantBits = 2;
	private const BindValueKind ValueKindSignificantBitsMask = unchecked((BindValueKind)~((1 << ValueKindInsignificantBits) - 1));

	[Flags]
	internal enum BindValueKind : ushort
	{
		RValue = 1 << ValueKindInsignificantBits,

		LValue = 2 << ValueKindInsignificantBits,

		Variable = 4 << ValueKindInsignificantBits,
	}

	private BoundExpression BindExpression(ExpressionSyntax syntax, DiagnosticBag diagnostics, bool invoked, bool indexed)
	{
		switch (syntax)
		{
			case LiteralExpressionSyntax literal:
				return BindLiteralConstant(literal, diagnostics);

			// case UnaryExpressionSyntax unary:
			// 	return BindUnaryExpression(unary, diagnostics);
			//
			// case BinaryExpressionSyntax binary:
			// 	return BindBinaryExpression(binary, diagnostics);

			case ParenthesizedExpressionSyntax paren:
				return BindExpression(paren.Expression, diagnostics, invoked: false, indexed: false);

			default:
				throw new Exception($"Unexpected syntax node {syntax.Kind}");
		}
	}

	private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax, DiagnosticBag diagnostics)
	{
		var left = BindRValueWithoutTargetType(syntax.Left, diagnostics);
		var right = BindRValueWithoutTargetType(syntax.Right, diagnostics);

		if (IsSimpleBinaryOperator(syntax.Kind))
		{
			return BindSimpleBinaryOperator(syntax, diagnostics, left, right);
		}

		throw new NotImplementedException();
	}

	private BoundExpression CheckValue(BoundExpression expression, BindValueKind valueKind, DiagnosticBag diagnostics)
	{
		var actual = expression.ValueKind;

		if ((actual & ValueKindSignificantBitsMask) == (valueKind & ValueKindSignificantBitsMask))
			return expression;

		// TODO: Error in diagnostic

		return expression;
	}

	private BoundExpression BindValue(ExpressionSyntax syntax, DiagnosticBag diagnostics, BindValueKind valueKind)
	{
		var result = this.BindExpression(syntax, diagnostics, invoked: false, indexed: false);
		return CheckValue(result, valueKind, diagnostics);
	}

	private BoundExpression BindRValueWithoutTargetType(ExpressionSyntax syntax, DiagnosticBag diagnostics)
	{
		return BindValue(syntax, diagnostics, BindValueKind.RValue);
	}

	private BoundLiteral BindLiteralConstant(LiteralExpressionSyntax syntax, DiagnosticBag diagnostics)
	{
		NumberToken? value = syntax.Token as NumberToken;
		Debug.Assert(value != null);

		return new BoundLiteral(syntax, (int)value.Value.U64, BuiltinTypeSymbol.I32);
	}
}