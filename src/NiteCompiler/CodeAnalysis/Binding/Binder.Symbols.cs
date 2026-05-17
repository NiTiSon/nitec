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

		if (syntax.Kind == NodeKind.IdentifierNameExpression)
		{
			return BindModuleOrTypeSymbol((IdentifierNameSyntax)syntax, null, diagnostics);
		}

		if (syntax.Kind == NodeKind.GenericNameExpression)
		{
			return BindGenericTypeSymbol((GenericNameSyntax)syntax, diagnostics);
		}

		if (syntax.Kind == NodeKind.ReferenceType)
		{
			return BindReferenceType((ReferenceTypeSyntax)syntax, diagnostics);
		}

		if (syntax.Kind == NodeKind.UnsizedArrayType)
		{
			return BindUnsizedArrayType((UnsizedArrayTypeSyntax)syntax, diagnostics);
		}

		if (syntax.Kind == NodeKind.ArrayType)
		{
			return BindArrayType((ArrayTypeSyntax)syntax, diagnostics);
		}

		throw new NotImplementedException();
	}

	private Symbol BindModuleOrTypeSymbol(IdentifierNameSyntax identifier, ContainerSymbol? container, BindingDiagnosticBag diagnostics)
	{
		string identifierText = identifier.GetName();

		var result = LookupResult.GetInstance();
		LookupOptions options = LookupOptions.ModulesOrTypesOnly;

		LookupSymbolsSimpleName(result, container, identifierText, 0, options, diagnose: true);

		Symbol bindingResult = ResultSymbol(result, identifierText, 0, identifier, diagnostics, out bool wasError, container, options);
		result.Free();
		return bindingResult;
	}

	private Symbol BindGenericTypeSymbol(GenericNameSyntax genericNameSyntax, BindingDiagnosticBag diagnostics)
	{
		throw new NotImplementedException("TODO: Implement lookup for generic types");
	}

	private TypeSymbol BindReferenceType(ReferenceTypeSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		TypeSymbol element = BindType(syntax.ElementSyntax, diagnostics);
		bool isMutable = syntax.ConstToken == null;
		bool isNullable = syntax.QuestionToken != null;
		return Compilation.CreateReferenceType(element, isMutable, isNullable);
	}

	private TypeSymbol BindUnsizedArrayType(UnsizedArrayTypeSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		TypeSymbol element = BindType(syntax.Type, diagnostics);
		return Compilation.CreateUnsizedArrayType(element);
	}

	private TypeSymbol BindArrayType(ArrayTypeSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		throw new NotImplementedException("Sized array is not implemented yet.");
	}

	public TypeSymbol BindType(ExpressionSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		Symbol symbol = BindModuleOrTypeSymbol(syntax, diagnostics);

		if (symbol is TypeSymbol typeSymbol) return typeSymbol;

		throw new NotImplementedException();
	}

	/// <summary>
	/// Returns eficient type to resolve operators, function parameters, conversions.
	/// </summary>
	/// <param name="from"></param>
	/// <returns></returns>
	private TypeSymbol GetEfficientType(TypeSymbol from)
	{
		return from switch
		{
			PointerTypeSymbol pointer => pointer.PointsTo,
			ReferenceTypeSymbol reference => reference.PointsTo,
			_ => from
		};
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