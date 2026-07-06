using System.Diagnostics;
using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Binding;

internal partial class Binder
{
	private BoundExpression BindInvocation(InvocationExpressionSyntax invocation, BindingDiagnosticBag diagnostics)
	{
		ImmutableArray<BoundExpression> arguments = BindInvocationArguments(invocation, diagnostics);

		if (invocation.Expression is not SimpleNameSyntax name)
		{
			diagnostics.Diagnostics.ReportUnresolvedSymbol(invocation.Expression.Location);
			return new BoundBadExpression(invocation, LookupResultKind.Empty, [], arguments, CreateErrorType());
		}

		LookupResult result = LookupResult.GetInstance();
		try
		{
			LookupIdentifier(result, name, invoked: true);

			if (result.Kind == LookupResultKind.Empty)
			{
				diagnostics.Diagnostics.ReportUnresolvedSymbol(name.Location);
				return new BoundBadExpression(invocation, LookupResultKind.Empty, [], arguments, CreateErrorType());
			}

			FunctionSymbol? function = ResolveInvokedFunction(result, arguments.Length);
			if (function == null)
			{
				diagnostics.Diagnostics.ReportUnresolvedSymbol(name.Location);
				return new BoundBadExpression(invocation, result.Kind, [..result.Symbols], arguments, CreateErrorType());
			}

			bool hasErrors = CheckInvocationArguments(function, arguments, diagnostics);
			return new BoundCall(invocation, function, arguments, hasErrors);
		}
		finally
		{
			result.Free();
		}
	}

	private ImmutableArray<BoundExpression> BindInvocationArguments(InvocationExpressionSyntax invocation,
		BindingDiagnosticBag diagnostics)
	{
		ArrayBuilder<BoundExpression> arguments = ArrayBuilder<BoundExpression>.GetInstance(
			invocation.ArgumentList.Arguments.Count);

		foreach (ExpressionSyntax argumentSyntax in invocation.ArgumentList.Arguments)
		{
			arguments.Add(BindRValueWithoutTargetType(argumentSyntax, diagnostics));
		}

		return arguments.ToImmutableAndFree();
	}

		private static FunctionSymbol? ResolveInvokedFunction(LookupResult result, int argumentCount)
		{
			foreach (Symbol symbol in result.Symbols)
			{
				if (symbol is not FunctionSymbol function)
				{
					continue;
				}

				if (function.Parameters.Length == argumentCount)
				{
					return function;
				}
			}

			return null;
		}

	private static bool CheckInvocationArguments(FunctionSymbol function, ImmutableArray<BoundExpression> arguments,
		BindingDiagnosticBag diagnostics)
	{
		Debug.Assert(function.Parameters.Length == arguments.Length);

		bool hasErrors = false;
		for (int i = 0; i < arguments.Length; i++)
		{
			BoundExpression argument = arguments[i];
			ParameterSymbol parameter = function.Parameters[i];

			if (argument.Type != parameter.Type)
			{
				diagnostics.Diagnostics.ReportCannotImplicitlyConvert(argument.Syntax!.Location, argument.Type, parameter.Type);
				hasErrors = true;
			}
		}

		return hasErrors;
	}
}
