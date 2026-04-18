using System;
using System.Collections.Immutable;
using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Binding;

internal partial class Binder
{
	private TypeSymbol GetSpecialType(SpecialType type, BindingDiagnosticBag diagnostics)
	{
		Debug.Assert(type != SpecialType.None);
		TypeSymbol? special = Compilation.GetSpecialType(type);

		if (special == null)
		{
			diagnostics.Diagnostics.ReportUnresolvedPredefinedType(type.ToFullName()!);
			special = CreateErrorType(type.ToFullName()!);
		}

		return special;
	}

	public Symbol BindVoidType(BindingDiagnosticBag diagnostics)
	{
		return GetSpecialType(SpecialType.StdVoid, diagnostics);
	}

	public Symbol BindPredefinedType(PredefinedTypeSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		return GetSpecialType(syntax.TypeKeyword.TKind.AssociatedSpecialType, diagnostics);
	}

	public Symbol BindModuleOrTypeSymbol(ExpressionSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		if (syntax.Kind == NodeKind.PredefinedType)
		{
			return BindPredefinedType((PredefinedTypeSyntax)syntax, diagnostics);
		}

		throw new NotImplementedException();
	}

	public TypeSymbol BindType(ExpressionSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		Symbol symbol = BindModuleOrTypeSymbol(syntax, diagnostics);

		if (symbol is TypeSymbol typeSymbol) return typeSymbol;

		throw new NotImplementedException();
	}

	private Symbol ResultSymbol(LookupResult result,
		string simpleName,
		int arity,
		SyntaxNode where,
		BindingDiagnosticBag diagnostics,
		out bool wasError,
		ContainerSymbol? container,
		LookupOptions options = default)
	{
		var symbols = result.Symbols;

		if (result.IsMultiViable)
		{
			if (symbols.Count > 1)
			{
				// symbols.Sort(ConsistentSymbolOrder.Instance);

				ImmutableArray<Symbol> originalSymbols = [..symbols];

			}

			wasError = false;
			return symbols[0];
		}

		wasError = true;
		Debug.Assert(symbols.Count > 0);
		return symbols[0];
	}
}