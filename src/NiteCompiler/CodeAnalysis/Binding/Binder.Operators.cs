using NiteCompiler.CodeAnalysis.Binding.Operators;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Binding;

internal partial class Binder
{
	protected static bool IsSimpleUnaryOperator(NodeKind kind)
	{
		if (kind == NodeKind.AddressOfExpression)
		{
			return false;
		}

		if (kind == NodeKind.DereferencingExpression)
		{
			return false;
		}

		return true;
	}

	private BoundExpression BindSimpleUnaryOperator(UnaryExpressionSyntax syntax, BindingDiagnosticBag diagnostics, BoundExpression expression)
	{
		UnaryOperatorKind operatorKind = syntax.Kind.UnaryOperator;
		UnaryOperatorSignature? resultOperator = null;
		if (expression.Type.SpecialType != SpecialType.None)
		{
			var result = ArrayBuilder<UnaryOperatorSignature>.GetInstance();
			Compilation.BuiltInOperators.GetOperators(operatorKind, result);

			if (result.Any())
			{
				foreach (UnaryOperatorSignature candidate in result)
				{
					if (candidate.InputType.Equals(expression.Type))
					{
						resultOperator = candidate;
						break;
					}
				}
			}

			result.Free();
		}

		// builtin operators can be built with null return type if returning type is not registered
		if (resultOperator?.ReturnType is null)
		{
			resultOperator = null;
		}

		resultOperator ??= new(operatorKind, expression.Type, CreateErrorType());
		return new BoundUnaryExpression(syntax, resultOperator.Value, expression);
	}

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
					if (candidate.LeftType.Equals(left.Type) && candidate.RightType.Equals(right.Type))
					{
						resultOperator = candidate;
						break;
					}
				}
			}

			result.Free();
		}

		// builtin operators can be built with null return type if returning type is not registered
		if (resultOperator?.ReturnType is null)
		{
			resultOperator = null;
		}

		resultOperator ??= new(operatorKind, left.Type, right.Type, CreateErrorType());
		return new BoundBinaryExpression(syntax, left, resultOperator.Value, right);
	}

	private BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		BoundExpression left = BindLValueWithoutTargetType(syntax.Left, diagnostics);
		BoundExpression right = BindToNaturalType(BindRValueWithoutTargetType(syntax.Right, diagnostics), left.Type, diagnostics);

		if (left.HasErrors || right.HasErrors)
		{
			return new BoundAssignment(syntax, left, right);
		}

		if (!right.Type.Equals(left.Type))
		{
			BoundConversion? conversion = ConvertImplicitly(right, left.Type, diagnostics);
			if (conversion != null)
			{
				right = conversion;
			}
			else
			{
				diagnostics.Diagnostics.ReportCannotImplicitlyConvert(right.Syntax!.Location, right.Type, left.Type);
				return new BoundAssignment(syntax, left, right, hasErrors: true);
			}
		}

		return new BoundAssignment(syntax, left, right);
	}

	private BoundExpression BindCompoundAssignmentExpression(AssignmentExpressionSyntax syntax,
		BindingDiagnosticBag diagnostics)
	{
		BoundExpression left = BindValue(syntax.Left, diagnostics, BindValueKind.LValue);
		BoundExpression right = BindValue(syntax.Right, diagnostics, BindValueKind.RValue);

		if (left.HasErrors || right.HasErrors)
		{
			return new BoundCompoundAssignment(syntax, BinaryOperatorSignature.Error, left, right, hasErrors: true);
		}

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
					if (candidate.LeftType.Equals(left.Type) && candidate.RightType.Equals(right.Type))
					{
						resultOperator = candidate;
						break;
					}
				}
			}

			result.Free();
		}

		if (resultOperator?.ReturnType is null)
		{
			resultOperator = null;
		}

		resultOperator ??= new(operatorKind, left.Type, right.Type, CreateErrorType());
		return new BoundCompoundAssignment(syntax, resultOperator.Value, left, right);
	}
}