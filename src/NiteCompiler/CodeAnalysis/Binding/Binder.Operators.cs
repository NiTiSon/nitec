using System;
using NiteCompiler.CodeAnalysis.Binding.BoundTree;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Binding;

internal partial class Binder
{
	protected static bool IsSimpleBinaryOperator(NodeKind kind)
	{
		if (kind == NodeKind.ConditionalAndExpression)
		{
			return false;
		}

		if (kind == NodeKind.ConditionalOrExpression)
		{
			return false;
		}

		return kind is { IsBinary: true, IsAssignmentExpression: false };
	}

	private BoundExpression BindSimpleBinaryOperator(BinaryExpressionSyntax syntax, DiagnosticBag diagnostics,
		BoundExpression left, BoundExpression right)
	{
		if (left.Type.SpecialType == SpecialType.StdNumericsSInt32 &&
		    right.Type.SpecialType == SpecialType.StdNumericsSInt32)
		{
			BinaryOperatorSignature op = new(BinaryOperatorKind.Addition, BuiltinTypeSymbol.I32, BuiltinTypeSymbol.I32, BuiltinTypeSymbol.I32);
			return new BoundBinaryExpression(syntax, left, op, right);
		}
		else
		{
			throw new NotImplementedException();
		}
	}
}