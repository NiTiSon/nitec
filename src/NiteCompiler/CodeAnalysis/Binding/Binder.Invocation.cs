using System.Diagnostics;
using System.Collections.Immutable;
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
		ImmutableArray<BoundExpression> arguments = BindInvocationArguments(invocation, diagnostics);

		if (invocation.Expression is not SimpleNameSyntax name)
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
				diagnostics.Diagnostics.ReportUnresolvedFunction(name.Location);
				return new BoundBadExpression(invocation, methodGroup.ResultKind, [], arguments, CreateErrorType());
			}

			FunctionSymbol? function = ResolveInvokedFunction(methodGroup.Candidates, arguments.Length);
			if (function == null)
			{
				diagnostics.Diagnostics.ReportUnresolvedFunction(name.Location);
				return new BoundBadExpression(invocation, methodGroup.ResultKind,
					[..methodGroup.Candidates], arguments, CreateErrorType());
			}

			bool hasErrors = CheckInvocationArguments(function, arguments, diagnostics);

			// TODO: make it more esthetic
			TypeSymbol? typeOverride = function is ConstructorSymbol ctor
				? ctor.ContainingType
				: null;

			return new BoundCall(invocation, function, arguments, typeOverride, hasErrors);
		}

		if (boundExpression.HasErrors)
		{
			return new BoundBadExpression(invocation, LookupResultKind.Empty, [], arguments, CreateErrorType());
		}

		diagnostics.Diagnostics.ReportUnresolvedSymbol(name.Location);
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

	private static FunctionSymbol? ResolveInvokedFunction(ImmutableArray<FunctionSymbol> candidates, int argumentCount)
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

	private static bool CheckInvocationArguments(FunctionSymbol function, ImmutableArray<BoundExpression> arguments,
		BindingDiagnosticBag diagnostics)
	{
		int offset = function is ConstructorSymbol ? 1 : 0;
		Debug.Assert(function.Parameters.Length - offset == arguments.Length);

		bool hasErrors = false;
		for (int i = 0; i < arguments.Length; i++)
		{
			BoundExpression argument = arguments[i];
			ParameterSymbol parameter = function.Parameters[i + offset];

			if (argument.Type != parameter.Type)
			{
				diagnostics.Diagnostics.ReportCannotImplicitlyConvert(argument.Syntax!.Location, argument.Type, parameter.Type);
				hasErrors = true;
			}
		}

		return hasErrors;
	}
}
