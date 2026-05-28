using System.Diagnostics;
using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Binding.OverloadResolution;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Binding;

internal partial class Binder
{
	private BoundExpression BindFunctionGroup(ExpressionSyntax node, bool invoked, bool indexed, BindingDiagnosticBag diagnostics)
	{
		NodeKind nodeKind = node.Kind;
		if (nodeKind == NodeKind.IdentifierNameExpression ||
		    nodeKind == NodeKind.GenericNameExpression)
		{
			return BindIdentifier((SimpleNameSyntax)node, invoked, indexed, diagnostics);
		}

		if (nodeKind == NodeKind.PathNameExpression)
		{
			return BindPath((PathNameSyntax)node, diagnostics, invoked, indexed);
		}

		if (nodeKind == NodeKind.MemberAccessExpression) // TODO: PLANNED PointerMemberAccessExpression
		{
			return BindMemberAccess((MemberAccessExpressionSyntax)node, diagnostics, invoked, indexed);
		}

		if (nodeKind == NodeKind.ParenthesizedExpression)
		{
			return BindFunctionGroup(((ParenthesizedExpressionSyntax)node).Expression, invoked, indexed, diagnostics);
		}

		return BindExpression(node, diagnostics, invoked, indexed);
	}


	private BoundExpression BindInvocation(InvocationExpressionSyntax invocation, BindingDiagnosticBag diagnostics)
	{
		// invoking
		// type() - constructor
		// type::constructor_name() - named constructor
		// func() - function
		// method() - method (require add implicit bound self)
		// self.method() - method
		// PLANNED:
		// new - auto constructor
		// new(args) - auto constructor with arguments
		// new named - auto named constructor
		// new named(args) - auto named constructor with arguments
		ImmutableArray<BoundExpression> arguments = BindInvocationArguments(invocation, diagnostics);

		if (invocation.Expression is not NameSyntax name)
		{
			diagnostics.Diagnostics.ReportUnresolvedSymbol(invocation.Expression.Location);
			return new BoundBadExpression(invocation, LookupResultKind.Empty, [], arguments, CreateErrorType());
		}

		BoundExpression boundExpression = BindFunctionGroup(invocation.Expression, invoked: true, indexed: false, diagnostics: diagnostics);
		boundExpression = CheckValue(boundExpression, BindValueKind.RValueOrFunctionGroup, diagnostics);

		if (boundExpression is BoundFunctionGroup methodGroup)
		{
			if (methodGroup.Candidates.Length == 0)
			{
				diagnostics.Diagnostics.ReportUnresolvedFunction(name.UnqualifiedName.Location);
				return new BoundBadExpression(invocation, methodGroup.ResultKind, [], arguments, CreateErrorType());
			}

			OverloadResolutionResult result = OverloadResolution.OverloadResolution.Resolve(methodGroup.Candidates, arguments);

			if (result.Status == OverloadResolutionStatus.Success)
			{
				FunctionSymbol bestFunction = result.BestFunction!;
				arguments = CheckInvocationArguments(bestFunction, arguments, diagnostics, out bool hasErrors);
				TypeSymbol? typeOverride = bestFunction is ConstructorSymbol ctor
					? ctor.ContainingType
					: null;

				return new BoundCall(invocation, bestFunction, arguments, typeOverride, hasErrors);
			}

			if (result.Status == OverloadResolutionStatus.Ambiguous)
			{
				diagnostics.Diagnostics.ReportOverloadResolutionFailure(
					name.UnqualifiedName.Location, methodGroup.Candidates[0].Name);

				FunctionSymbol? fallback = FindFirstMatchingArity(methodGroup.Candidates, arguments.Length);
				if (fallback != null)
				{
					arguments = CheckInvocationArguments(fallback, arguments, diagnostics, out _);
				}

				return new BoundBadExpression(invocation, LookupResultKind.OverloadResolutionFailure,
					[..methodGroup.Candidates], arguments, CreateErrorType());
			}

			{
				FunctionSymbol? fallback = FindFirstMatchingArity(methodGroup.Candidates, arguments.Length);
				if (fallback != null)
				{
					arguments = CheckInvocationArguments(fallback, arguments, diagnostics, out _);
					return new BoundBadExpression(invocation, LookupResultKind.OverloadResolutionFailure,
						[..methodGroup.Candidates], arguments, CreateErrorType());
				}

				diagnostics.Diagnostics.ReportUnresolvedFunction(name.UnqualifiedName.Location);
				return new BoundBadExpression(invocation, methodGroup.ResultKind,
					[..methodGroup.Candidates], arguments, CreateErrorType());
			}
		}

		if (boundExpression.HasErrors)
		{
			return new BoundBadExpression(invocation, LookupResultKind.Empty, [], arguments, CreateErrorType());
		}

		diagnostics.Diagnostics.ReportUnresolvedSymbol(name.UnqualifiedName.Location);
		return new BoundBadExpression(invocation, LookupResultKind.Empty, [], arguments, CreateErrorType());
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

	private static FunctionSymbol? FindFirstMatchingArity(ImmutableArray<FunctionSymbol> candidates, int argumentCount)
	{
		foreach (FunctionSymbol function in candidates)
		{
			int paramCount = function is ConstructorSymbol
				? function.Parameters.Length - 1
				: function.Parameters.Length;
			if (paramCount == argumentCount)
			{
				return function;
			}
		}

		return null;
	}

	private static ImmutableArray<BoundExpression> CheckInvocationArguments(FunctionSymbol function, ImmutableArray<BoundExpression> arguments,
		BindingDiagnosticBag diagnostics, out bool hasErrors)
	{
		int offset = function is ConstructorSymbol ? 1 : 0;
		Debug.Assert(function.Parameters.Length - offset == arguments.Length);

		bool anyErrors = false;
		ImmutableArray<BoundExpression>.Builder? argumentsBuilder = null;
		for (int i = 0; i < arguments.Length; i++)
		{
			BoundExpression argument = arguments[i];
			ParameterSymbol parameter = function.Parameters[i + offset];

			if (!argument.Type.Equals(parameter.Type))
			{
				BoundConversion? conversion = ConvertImplicitly(argument, parameter.Type, diagnostics);
				if (conversion != null)
				{
					if (argumentsBuilder == null)
					{
						argumentsBuilder = ImmutableArray.CreateBuilder<BoundExpression>(arguments.Length);
						argumentsBuilder.AddRange(arguments[..i]);
					}

					argumentsBuilder.Add(conversion);
				}
				else
				{
					diagnostics.Diagnostics.ReportCannotImplicitlyConvert(argument.Syntax!.Location, argument.Type, parameter.Type);
					anyErrors = true;

					if (argumentsBuilder != null)
					{
						argumentsBuilder.Add(argument);
					}
				}
			}
			else if (argumentsBuilder != null)
			{
				argumentsBuilder.Add(argument);
			}
		}

		hasErrors = anyErrors;
		return argumentsBuilder?.ToImmutable() ?? arguments;
	}
}
