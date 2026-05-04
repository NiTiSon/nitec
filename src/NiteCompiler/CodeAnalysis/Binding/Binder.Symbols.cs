using System;
using System.Collections.Immutable;
using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Binding;

internal partial class Binder
{
	public TypeSymbol GetSpecialType(SpecialType specialType)
	{
		return Compilation.GetSpecialType(specialType);
	}

	public Symbol BindVoidType()
	{
		return GetSpecialType(SpecialType.StdVoid);
	}

	public Symbol BindPredefinedType(PredefinedTypeSyntax syntax)
	{
		return GetSpecialType(syntax.TypeKeyword.TKind.AssociatedSpecialType);
	}

	public Symbol BindModuleOrTypeSymbol(ExpressionSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		if (syntax.Kind == NodeKind.PredefinedType)
		{
			return BindPredefinedType((PredefinedTypeSyntax)syntax);
		}

		if (syntax.Kind == NodeKind.ReferenceType)
		{
			return BindReferenceType((ReferenceTypeSyntax)syntax, diagnostics);
		}

		throw new NotImplementedException();
	}

	private TypeSymbol BindReferenceType(ReferenceTypeSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		TypeSymbol element = BindType(syntax.ElementSyntax, diagnostics);
		bool isMutable = syntax.ConstToken == null;
		bool isNullable = syntax.QuestionToken != null;
		return Compilation.CreateReferenceType(element, isMutable, isNullable);
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