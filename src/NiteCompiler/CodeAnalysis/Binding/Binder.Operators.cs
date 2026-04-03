using System;
using System.Diagnostics;
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

	protected static bool IsAssignmentBinaryOperator(NodeKind kind)
	{
		if (kind == NodeKind.ConditionalAndExpression)
		{
			return false;
		}

		if (kind == NodeKind.ConditionalOrExpression)
		{
			return false;
		}

		return kind is { IsBinary: true, IsAssignmentExpression: true };
	}

	private BoundExpression BindSimpleBinaryOperator(BinaryExpressionSyntax syntax, BindingDiagnosticBag diagnostics,
		BoundExpression left, BoundExpression right)
	{
		BinaryOperatorKind operatorKind = syntax.Kind.BinaryOperator;
		if (left.Type.SpecialType == SpecialType.StdNumericsSInt32 &&
		    right.Type.SpecialType == SpecialType.StdNumericsSInt32)
		{
			TypeSymbol i32 = GetSpecialType(SpecialType.StdNumericsSInt32, diagnostics);
			BinaryOperatorSignature op = new(operatorKind, i32, i32, i32);
			return new BoundBinaryExpression(syntax, left, op, right);
		}

		// TODO: Errors
		BinaryOperatorSignature opErr = new(operatorKind, left.Type, right.Type, CreateErrorType());
		return new BoundBinaryExpression(syntax, left, opErr, right);
	}

	private BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		BoundExpression left = BindLValueWithoutTargetType(syntax.Left, diagnostics);
		BoundExpression right = BindRValueWithoutTargetType(syntax.Right, diagnostics);

		// TODO: Conversion
		// TODO: Errors

		return new BoundAssignment(syntax, left, right);
	}
}