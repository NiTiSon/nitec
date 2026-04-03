using System;
using System.Data.SqlTypes;
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
		BinaryOperatorSignature? resultOperator = null;
		if (left.Type.SpecialType != SpecialType.None && right.Type.SpecialType != SpecialType.None)
		{
			var result = ArrayBuilder<BinaryOperatorSignature>.GetInstance();
			Compilation.BuiltInOperators.GetOperators(operatorKind, result);

			if (result.Any())
			{
				foreach (BinaryOperatorSignature candidate in result)
				{
					if (candidate.LeftType == left.Type && candidate.RightType == right.Type)
					{
						resultOperator = candidate;
						break;
					}
				}
			}

			result.Free();
		}

		// builtin operators can be built with null return type if returning type is not registered
		if (resultOperator?.ReturnType == null)
		{
			resultOperator = null;
		}

		resultOperator ??= new(operatorKind, left.Type, right.Type, CreateErrorType());
		return new BoundBinaryExpression(syntax, left, resultOperator.Value, right);
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