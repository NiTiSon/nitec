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

		LookupResult result = LookupResult.GetInstance();
		try
		{
			BoundExpression boundExpression = BindFunctionGroup(invocation.Expression, invoked: true, indexed: false, diagnostics: diagnostics);
			boundExpression = CheckValue(boundExpression, BindValueKind.RValueOrFunctionGroup, diagnostics);

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
