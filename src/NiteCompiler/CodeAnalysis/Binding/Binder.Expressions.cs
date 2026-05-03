using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Binding;

internal partial class Binder
{
	private const int ValueKindInsignificantBits = 2;
	private const BindValueKind ValueKindSignificantBitsMask = unchecked((BindValueKind)~((1 << ValueKindInsignificantBits) - 1));

	[Flags]
	internal enum BindValueKind : ushort
	{
		RValue = 1 << ValueKindInsignificantBits,

		LValue = 2 << ValueKindInsignificantBits,

		Variable = 4 << ValueKindInsignificantBits,
	}

	private BoundBadExpression BadExpression(SyntaxNode syntax)
	{
		return BadExpression(syntax, LookupResultKind.Empty, []);
	}

	private BoundBadExpression BadExpression(SyntaxNode syntax, LookupResultKind resultKind, ImmutableArray<Symbol> symbols)
	{
		return new BoundBadExpression(syntax,
			resultKind,
			symbols,
			ImmutableArray<BoundExpression>.Empty,
			CreateErrorType());
	}

	internal TypeSymbol CreateErrorType(string name = "")
	{
		return new ErrorTypeSymbol(Compilation, SpecialType.None, name, arity: 0, errorInfo: null, unreported: false);
	}

	internal BoundExpression BindExpression(ExpressionSyntax syntax, BindingDiagnosticBag diagnostics, bool invoked, bool indexed)
	{
		switch (syntax)
		{
			case LiteralExpressionSyntax literal:
				return BindLiteralConstant(literal, diagnostics);

			case AssignmentExpressionSyntax assignment:
				if (assignment.Kind == NodeKind.AssignmentExpression)
				{
					return BindAssignmentExpression(assignment, diagnostics);
				}

				return BindCompoundAssignmentExpression(assignment, diagnostics);

			case UnaryExpressionSyntax unary:
				return BindUnaryExpression(unary, diagnostics);

			case BinaryExpressionSyntax binary:
				return BindBinaryExpression(binary, diagnostics);

			case ParenthesizedExpressionSyntax paren:
				return BindExpression(paren.Expression, diagnostics, invoked: false, indexed: false);

			case SimpleNameSyntax name:
				return BindIdentifier(name, invoked, indexed, diagnostics);
			case InvocationExpressionSyntax invocation:
				return BindInvocation(invocation, diagnostics);

			default:
				throw new UnreachableException($"BindExpression({syntax.Kind})");
		}
	}

	private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		var expression = BindRValueWithoutTargetType(syntax.Expression, diagnostics);

		if (IsSimpleUnaryOperator(syntax.Kind))
		{
			return BindSimpleUnaryOperator(syntax, diagnostics, expression);
		}

		throw new NotImplementedException("Pointer operators are not implemented yet.");
	}

	private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		var left = BindRValueWithoutTargetType(syntax.Left, diagnostics);
		var right = BindRValueWithoutTargetType(syntax.Right, diagnostics);

		if (IsSimpleBinaryOperator(syntax.Kind))
		{
			return BindSimpleBinaryOperator(syntax, diagnostics, left, right);
		}

		throw new NotImplementedException("Operators && and || not implemented yet.");
	}

	private BoundExpression CheckValue(BoundExpression expression, BindValueKind valueKind, BindingDiagnosticBag diagnostics)
	{
		var actual = expression.ValueKind;

		if ((actual & ValueKindSignificantBitsMask) == (valueKind & ValueKindSignificantBitsMask))
			return expression;

		// TODO: Error in diagnostic

		return expression;
	}

	private BoundExpression BindValue(ExpressionSyntax syntax, BindingDiagnosticBag diagnostics, BindValueKind valueKind)
	{
		var result = this.BindExpression(syntax, diagnostics, invoked: false, indexed: false);
		return CheckValue(result, valueKind, diagnostics);
	}

	private BoundExpression BindLValueWithoutTargetType(ExpressionSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		return BindValue(syntax, diagnostics, BindValueKind.LValue);
	}

	private BoundExpression BindRValueWithoutTargetType(ExpressionSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		return BindValue(syntax, diagnostics, BindValueKind.RValue);
	}

	private BoundExpression BindBooleanExpression(ExpressionSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		BoundExpression result = BindExpression(syntax, diagnostics, false, false);

		if (result.Type.SpecialType != SpecialType.StdBoolean)
		{
			diagnostics.Diagnostics.ReportCannotImplicitlyConvert(syntax.Location, result.Type, GetSpecialType(SpecialType.StdBoolean));

			// TODO: Wrap expression in wrong conversion with hasError = true
		}

		return result;
	}

	private BoundLiteral BindLiteralConstant(LiteralExpressionSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		if (syntax.Kind == NodeKind.FalseLiteralExpression || syntax.Kind == NodeKind.TrueLiteralExpression)
		{
			return BindBooleanLiteralConstant(syntax, diagnostics);
		}

		if (syntax.Kind == NodeKind.NumberLiteralExpression)
		{
			return BindNumericLiteralExpression(syntax, diagnostics);
		}

		if (syntax.Kind == NodeKind.CharacterLiteralExpression)
		{
			return BindCharacterLiteralExpression(syntax, diagnostics);
		}

		if (syntax.Kind == NodeKind.StringLiteralExpression)
		{
			return BindStringLiteralExpression(syntax, diagnostics);
		}

		throw new UnreachableException($"BindLiteralConstant({syntax.Kind})");
	}

	private BoundLiteral BindBooleanLiteralConstant(LiteralExpressionSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		Debug.Assert(syntax.Kind == NodeKind.FalseLiteralExpression ||
		             syntax.Kind == NodeKind.TrueLiteralExpression);

		TypeSymbol booleanType = GetSpecialType(SpecialType.StdBoolean);
		if (syntax.Kind == NodeKind.FalseLiteralExpression)
		{
			return new BoundLiteral(syntax, ConstantValue.Create(false), booleanType);
		}

		return new BoundLiteral(syntax, ConstantValue.Create(true), booleanType);
	}

	private BoundLiteral BindNumericLiteralExpression(LiteralExpressionSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		Debug.Assert(syntax.Kind == NodeKind.NumberLiteralExpression);
		NumberToken? value = syntax.Token as NumberToken;
		Debug.Assert(value != null);

		// TODO: Fully implement
		TypeSymbol i32 = GetSpecialType(SpecialType.StdNumericsSInt32);
		ConstantValue i32Value = ConstantValue.Create((int)value.Value.U64);
		return new BoundLiteral(syntax, i32Value, i32);
	}

	private BoundLiteral BindCharacterLiteralExpression(LiteralExpressionSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		Debug.Assert(syntax.Kind == NodeKind.CharacterLiteralExpression);
		StringToken content = (syntax.Token as StringToken)!;
		SpecialType type = content.LiteralType switch
		{
			StringLiteralType.Unicode8 => SpecialType.StdTextCharacterUtf8,
			StringLiteralType.Unicode16 => SpecialType.StdTextCharacterUtf16,
			StringLiteralType.Unicode32 => SpecialType.StdTextCharacterUtf32,
			_ => SpecialType.StdTextCharacterUtf8
		};
		return new BoundLiteral(syntax, ConstantValue.Create(0), GetSpecialType(type));
	}

	private BoundLiteral BindStringLiteralExpression(LiteralExpressionSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		// TODO: Implement
		return new BoundLiteral(syntax, ConstantValue.Create(0), GetSpecialType(SpecialType.StdVoid));
	}

	private BoundExpression BindIdentifier(SimpleNameSyntax name, bool invoked, bool indexed,
		BindingDiagnosticBag diagnostics)
	{
		LookupResult result = LookupResult.GetInstance();
		string identifierName = name.GetName();
		LookupIdentifier(result, name, invoked);

		BoundExpression boundExpression;
		if (result.Kind == LookupResultKind.Empty)
		{
			diagnostics.Diagnostics.ReportUnresolvedSymbol(name.Location);
			boundExpression = BadExpression(name);
		}
		else
		{
			var group = ArrayBuilder<Symbol>.GetInstance();
			Symbol? symbol = GetSymbolOrFunctionGroup(result, name, identifierName, arity: 0, group,  diagnostics, out bool isError);

			if (symbol is null) // function group
			{
				Debug.Assert(group.Count > 0);

				throw new NotImplementedException();
			}
			else
			{
				boundExpression = BindNonFunction(name, symbol, diagnostics, result.Kind, indexed, isError);
			}
			group.Free();
		}

		result.Free();
		return boundExpression;
	}

	private BoundExpression BindNonFunction(SimpleNameSyntax name, Symbol symbol, BindingDiagnosticBag diagnostics, LookupResultKind resultKind, bool indexed, bool wasError)
	{
		switch (symbol.Kind)
		{
			case SymbolKind.LocalVariable:
				return new BoundLocal(name, (LocalVariableSymbol)symbol);
				break;
			case SymbolKind.Parameter:
				return new BoundParameter(name, (ParameterSymbol)symbol);
			default:
				throw new UnreachableException();
		}
	}

	private Symbol? GetSymbolOrFunctionGroup(LookupResult result, SyntaxNode node, string identifierName, int arity, ArrayBuilder<Symbol> methodGroup, BindingDiagnosticBag diagnostics, out bool wasError)
	{
		Debug.Assert(methodGroup.Count == 0);
		wasError = false;

		Symbol? other = null;
		foreach (Symbol symbol in result.Symbols)
		{
			var kind = symbol.Kind;
			if (methodGroup.Count > 0)
			{
				var existingKind = methodGroup[0].Kind;
				if (existingKind != kind)
				{
					if ((existingKind == SymbolKind.Function) ||
					    (existingKind == SymbolKind.Property && kind != SymbolKind.Function))
					{
						other = symbol;
						continue;
					}

					other = methodGroup[0];
					methodGroup.Clear();
				}
			}

			if (kind is SymbolKind.Function or SymbolKind.Property)
			{
				methodGroup.Add(symbol);
			}
			else
			{
				other = symbol;
			}
		}

		Debug.Assert(methodGroup.Count != 0 || other != null);

		if ((methodGroup.Count > 0) &&
		    IsFunctionGroup(methodGroup))
		{
			if ((methodGroup[0].Kind == SymbolKind.Function) || other == null)
			{
				if (result.Error != null)
				{
					diagnostics.Diagnostics.Add(result.Error);
					wasError = (result.Error.Severity == DiagnosticSeverity.Error);
				}

				return null;
			}
		}

		methodGroup.Clear();
		return ResultSymbol(result, identifierName, arity, node, diagnostics, out wasError, null);
	}

	private static bool IsFunctionGroup(ArrayBuilder<Symbol> members)
	{
		Debug.Assert(members.Count > 0);

		var member = members[0];

		// Members should be a consistent type.
		Debug.Assert(members.All(m => m.Kind == member.Kind));

		switch (member.Kind)
		{
			case SymbolKind.Function:
				return true;

			// case SymbolKind.Property:
			// 	Debug.Assert(members.All(m => !m.IsIndexer()));
			//
			// 	foreach (PropertySymbol property in members)
			// 	{
			// 		if (property.IsIndexedProperty)
			// 		{
			// 			return true;
			// 		}
			// 	}
			// 	return false;
			default:
				throw new UnreachableException();
		}
	}
}
