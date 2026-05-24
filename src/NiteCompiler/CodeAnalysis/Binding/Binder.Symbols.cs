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

		if (syntax.Kind == NodeKind.PathNameExpression)
		{
			return BindModuleOrTypeSymbol((PathNameSyntax)syntax, diagnostics);
		}

		throw new NotImplementedException();
	}

	private Symbol BindModuleOrTypeSymbol(PathNameSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		ContainerSymbol? container = ResolveQualifier(syntax.Left, diagnostics);

		if (container == null)
		{
			return CreateErrorType(syntax.UnqualifiedName.GetName());
		}

		string rightName = syntax.Right.GetName();

		LookupResult result = LookupResult.GetInstance();

		LookupMembersInternal(result, container, rightName, syntax.UnqualifiedName.Arity,
			LookupOptions.ModulesOrTypesOnly, this, diagnose: true);
		Debug.Assert(result.Kind != LookupResultKind.NotAnAttribute);

		switch (result.Kind)
		{
			case LookupResultKind.Empty:
				diagnostics.Diagnostics.ReportUnresolvedSymbol(syntax.Right.Location);
				return CreateErrorType(syntax.UnqualifiedName.GetName());

			case LookupResultKind.NotAModuleNorAType:
				diagnostics.Diagnostics.ReportSymbolIsNotAModuleNorAType(syntax.Right.Location, rightName);
				return CreateErrorType(syntax.UnqualifiedName.GetName());

			case LookupResultKind.Ambiguous:
				diagnostics.Diagnostics.ReportAmbiguousReference(syntax.Right.Location, rightName);
				return CreateErrorType(syntax.UnqualifiedName.GetName());

			case LookupResultKind.NotCreatable:
				diagnostics.Diagnostics.ReportSymbolIsNotCreatable(syntax.Right.Location, rightName);
				return CreateErrorType(syntax.UnqualifiedName.GetName());

			case LookupResultKind.Inaccessible:
				diagnostics.Diagnostics.ReportSymbolIsInaccessible(syntax.Right.Location, rightName);
				return CreateErrorType(syntax.UnqualifiedName.GetName());

			default:
			{
				// Viable or other intermediate kinds
				return result.Symbols[0];
			}
		}

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

		LifetimeSymbol? lifetime = null;
		if (syntax.Lifetime != null)
		{
			if (syntax.Lifetime.Identifier == "static")
			{
				lifetime = Compilation.GetStaticLifetime();
			}
			else
			{
				lifetime = LookupLifetimeSymbol(syntax.Lifetime, diagnostics);
			}
		}

		return Compilation.CreateReferenceType(element, isMutable, isNullable, lifetime);
	}

	private LifetimeSymbol? LookupLifetimeSymbol(LifetimeSyntax lifetimeSyntax, BindingDiagnosticBag diagnostics)
	{
		string name = lifetimeSyntax.Identifier;

		var result = LookupResult.GetInstance();
		LookupSymbolsSimpleName(result, null, name, 0, LookupOptions.Default, diagnose: true);

		Symbol symbol = ResultSymbol(result, name, 0, lifetimeSyntax, diagnostics, out _, null, LookupOptions.Default);
		result.Free();

		if (symbol is LifetimeSymbol lifetime)
		{
			return lifetime;
		}

		return null;
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

		if (symbol is TypeSymbol typeSymbol)
		{
			if (typeSymbol.IsErrorSymbol)
			{
				diagnostics.Diagnostics.ReportUnresolvedSymbol(syntax.Location);
			}

			return typeSymbol;
		}

		throw new NotImplementedException();
	}

	/// <summary>
	/// Returns efficient type to resolve operators, function parameters, conversions.
	/// </summary>
	private TypeSymbol GetEfficientType(TypeSymbol from, out bool isPointerAccessRequired)
	{
		isPointerAccessRequired = false;
		switch (from)
		{
			case PointerTypeSymbol pointer:
				isPointerAccessRequired = true;
				return pointer.PointsTo;
			case ReferenceTypeSymbol reference:
				return reference.PointsTo;
			default:
				return from;
		}
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

		if (result.Kind == LookupResultKind.Empty)
		{
			diagnostics.Diagnostics.ReportUnresolvedSymbol(where.Location);
			wasError = true;
			return CreateErrorType(simpleName);
		}

		switch (result.Kind)
		{
			case LookupResultKind.NotAModuleNorAType:
				diagnostics.Diagnostics.ReportSymbolIsNotAModuleNorAType(where.Location, simpleName);
				break;
			case LookupResultKind.NotAnAttribute:
				diagnostics.Diagnostics.ReportSymbolIsNotAnAttribute(where.Location, simpleName);
				break;
			case LookupResultKind.WrongArity:
				diagnostics.Diagnostics.ReportWrongTypeArity(where.Location, simpleName);
				break;
			case LookupResultKind.NotCreatable:
				diagnostics.Diagnostics.ReportSymbolIsNotCreatable(where.Location, simpleName);
				break;
			case LookupResultKind.Inaccessible:
				diagnostics.Diagnostics.ReportSymbolIsInaccessible(where.Location, simpleName);
				break;
			case LookupResultKind.NotAValue:
				diagnostics.Diagnostics.ReportSymbolIsNotAValue(where.Location, simpleName);
				break;
			case LookupResultKind.NotInvocable:
				diagnostics.Diagnostics.ReportSymbolIsNotInvocable(where.Location, simpleName);
				break;
			case LookupResultKind.OverloadResolutionFailure:
				diagnostics.Diagnostics.ReportOverloadResolutionFailure(where.Location, simpleName);
				break;
			case LookupResultKind.Ambiguous:
				diagnostics.Diagnostics.ReportAmbiguousReference(where.Location, simpleName);
				break;
		}

		wasError = true;
		Debug.Assert(symbols.Count > 0);
		return symbols[0];
	}
}