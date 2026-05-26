using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using NiteCompiler.CodeAnalysis.Binding.Conversions;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Binding;

internal partial class Binder
{
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

	internal TypeSymbol CreateErrorType(string name = "<error_type>")
	{
		return new ErrorTypeSymbol(Compilation, SpecialType.None, name, lifetimeArity: 0, arity: 0, errorInfo: null, unreported: false);
	}

	internal FieldSymbol CreateErrorField(ContainerSymbol owner, string name = "<error_field>")
	{
		Debug.Assert(owner != null);
		return new ErrorFieldSymbol(owner, null ?? CreateErrorType(), name, null, false, [], LookupResultKind.Empty);
	}

	internal FieldSymbol CreateErrorField(string name = "<error_field>")
	{
		return new ErrorFieldSymbol(Compilation, null, name, null, false);
	}

	internal BoundExpression BindExpression(ExpressionSyntax syntax, BindingDiagnosticBag diagnostics,
		bool invoked = false, bool indexed = false)
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
				return BindExpression(paren.Expression, diagnostics);

			case SimpleNameSyntax name:
				return BindIdentifier(name, invoked, indexed, diagnostics);
			case InvocationExpressionSyntax invocation:
				return BindInvocation(invocation, diagnostics);
			case IndexationExpressionSyntax indexation:
				throw new NotImplementedException("TODO[high]");

			case SelfExpressionSyntax selfExpr:
				return BindSelfExpression(selfExpr, diagnostics);
			case MemberAccessExpressionSyntax memberAccess:
				return BindMemberAccess(memberAccess, diagnostics);
			case PathNameSyntax path:
				return BindPath(path, diagnostics);
			case CastExpressionSyntax cast:
				return BindCastExpression(cast, diagnostics);

			default:
				throw new UnreachableException($"BindExpression({syntax.Kind})");
		}
	}

	private BoundExpression BindSelfExpression(SelfExpressionSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		LookupResult result = LookupResult.GetInstance();
		LookupIdentifier(result, "self", arity: 0, invoked: false);

		BoundExpression boundExpression;
		if (result.Kind == LookupResultKind.Empty)
		{
			diagnostics.Diagnostics.ReportUnresolvedSymbol(syntax.Location);
			boundExpression = BadExpression(syntax);
		}
		else
		{
			Symbol symbol = result.Symbols[0];

			switch (symbol.Kind)
			{
				case SymbolKind.LocalVariable:
				{
					var local = (LocalVariableSymbol)symbol;
					boundExpression = new BoundLocalVariable(syntax, local);
					break;
				}
				case SymbolKind.Parameter:
				{
					var param = (ParameterSymbol)symbol;
					boundExpression = new BoundParameter(syntax, param);
					break;
				}
				default:
					boundExpression = BadExpression(syntax);
					break;
			}
		}

		result.Free();
		return boundExpression;
	}

	private BoundExpression BindMemberAccess(MemberAccessExpressionSyntax syntax, BindingDiagnosticBag diagnostics,
		bool invoked = false, bool indexed = false)
	{
		BoundExpression receiver = BindExpression(syntax.Expression, diagnostics);

		if (receiver.HasErrors || receiver.Type.IsErrorSymbol)
		{
			return new BoundFieldAccess(syntax, receiver, CreateErrorField(receiver.Type, syntax.Name.GetName()), hasErrors: true);
		}

		string fieldName = syntax.Name.GetName();

		TypeSymbol effectiveType = GetEfficientType(receiver.Type, out bool accessThroughPointer);

		// TODO: replace with lookup?
		if (effectiveType is NamedTypeSymbol namedType)
		{
			foreach (var member in namedType.GetMembers(fieldName))
			{
				if (member is FieldSymbol field)
				{
					return new BoundFieldAccess(syntax, receiver, field);
				}
			}
		}

		diagnostics.Diagnostics.ReportUnresolvedSymbol(syntax.Name.Location);
		return new BoundFieldAccess(syntax, receiver, CreateErrorField(receiver.Type, syntax.Name.GetName()), hasErrors: true);
	}

	private BoundExpression BindPath(PathNameSyntax syntax, BindingDiagnosticBag diagnostics,
		bool invoked = false, bool indexed = false)
	{
		ContainerSymbol? container = ResolveQualifier(syntax.Left, diagnostics);

		if (container == null)
		{
			return BadExpression(syntax);
		}

		string rightName = syntax.Right.GetName();

		LookupResult result = LookupResult.GetInstance();
		LookupOptions options = LookupOptions.Default;
		if (invoked)
		{
			options |= LookupOptions.MustBeInvocableIfMember;
		}

		LookupMembersInternal(result, container, rightName, syntax.Right.Arity, options, this, diagnose: true);

		BoundExpression boundExpression;
		if (result.Kind == LookupResultKind.Empty)
		{
			diagnostics.Diagnostics.ReportUnresolvedSymbol(syntax.Right.Location);
			boundExpression = BadExpression(syntax);
		}
		else
		{
			var group = ArrayBuilder<Symbol>.GetInstance();
			Symbol? symbol = GetSymbolOrFunctionGroup(result, syntax, rightName, arity: 0, group, diagnostics, out bool isError);

			if (symbol is null)
			{
				Debug.Assert(group.Count > 0);

				var candidates = new FunctionSymbol[group.Count];
				for (int i = 0; i < group.Count; i++)
					candidates[i] = (FunctionSymbol)group[i];

				boundExpression = new BoundFunctionGroup(syntax, [..candidates], receiver: null, result.Kind, CreateErrorType());
			}
			else if (symbol is NamedTypeSymbol typeSymbol && invoked)
			{
				ImmutableArray<Symbol> members = typeSymbol.GetMembers();
				var constructors = ArrayBuilder<FunctionSymbol>.GetInstance();
				foreach (Symbol member in members)
				{
					if (member is ConstructorSymbol ctor)
					{
						constructors.Add(ctor);
					}
				}

				if (constructors.Count > 0)
				{
					boundExpression = new BoundFunctionGroup(syntax, constructors.ToImmutableAndFree(),
						receiver: null, result.Kind, CreateErrorType());
				}
				else
				{
					constructors.Free();
					boundExpression = BindNonFunction(syntax, symbol, diagnostics, result.Kind, indexed, isError);
				}
			}
			else
			{
				boundExpression = BindNonFunction(syntax, symbol, diagnostics, result.Kind, indexed, isError);
			}
			group.Free();
		}

		result.Free();
		return boundExpression;
	}

	private ContainerSymbol? ResolveQualifier(NameSyntax name, BindingDiagnosticBag diagnostics)
	{
		if (name is SimpleNameSyntax simpleName)
		{
			Symbol symbol = BindModuleOrTypeSymbol(simpleName, diagnostics);
			return symbol as ContainerSymbol;
		}

		if (name is PathNameSyntax pathName)
		{
			ContainerSymbol? leftContainer = ResolveQualifier(pathName.Left, diagnostics);
			if (leftContainer == null)
			{
				return null;
			}

			string rightName = pathName.Right.GetName();
			LookupResult result = LookupResult.GetInstance();
			LookupMembersInternal(result, leftContainer, rightName, arity: 0, LookupOptions.ModulesOrTypesOnly, this, diagnose: true);

			ContainerSymbol? container = null;
			if (result.Kind == LookupResultKind.Viable && result.Symbols.Count > 0)
			{
				container = result.Symbols[0] as ContainerSymbol;
			}

			if (container == null)
			{
				diagnostics.Diagnostics.ReportUnresolvedSymbol(pathName.Right.Location);
			}

			result.Free();
			return container;
		}

		return null;
	}

	private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		if (syntax.Kind == NodeKind.AddressOfExpression)
		{
			return BindAddressOfExpression(syntax, diagnostics);
		}

		if (syntax.Kind == NodeKind.DereferencingExpression)
		{
			return BindDereferenceExpression(syntax, diagnostics);
		}

		var expression = BindRValueWithoutTargetType(syntax.Expression, diagnostics);

		if (IsSimpleUnaryOperator(syntax.Kind))
		{
			return BindSimpleUnaryOperator(syntax, diagnostics, expression);
		}

		throw new NotImplementedException("Pointer operators are not implemented yet.");
	}

	private BoundExpression BindAddressOfExpression(UnaryExpressionSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		Debug.Assert(syntax.Kind == NodeKind.AddressOfExpression);
		BoundExpression operand = BindLValueWithoutTargetType(syntax.Expression, diagnostics);

		// TODO[high]: isMutable must be detected
		TypeSymbol refType = Compilation.CreateReferenceType(operand.Type, isMutable: true, isNullable: false);
		return new BoundAddressOfExpression(syntax, operand, refType);
	}

	private BoundExpression BindDereferenceExpression(UnaryExpressionSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		Debug.Assert(syntax.Kind == NodeKind.DereferencingExpression);
		BoundExpression operand = BindRValueWithoutTargetType(syntax.Expression, diagnostics);

		if (operand.Type is BaseReferenceTypeSymbol referenceType)
		{
			return new BoundDereferenceExpression(syntax, operand, referenceType.PointsTo, referenceType.IsMutable);
		}

		if (!operand.HasErrors && !operand.Type.IsErrorSymbol)
		{
			diagnostics.Diagnostics.ReportCannotDereferenceNonReference(syntax.Operator.Location, operand.Type);
		}

		return new BoundDereferenceExpression(syntax, operand, CreateErrorType(), isMutable: true, hasErrors: true);
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

	private BoundExpression BindValue(ExpressionSyntax syntax, BindingDiagnosticBag diagnostics, BindValueKind valueKind)
	{
		BoundExpression result = this.BindExpression(syntax, diagnostics);
		result = CheckValue(result, valueKind, diagnostics);
		if (valueKind == BindValueKind.RValue)
		{
			LocalVariableOrParameterSymbol? variable = result switch
			{
				BoundParameter p => p.Variable,
				BoundLocalVariable l => l.Variable,
				_ => null
			};

			if (variable != null)
			{
				return variable.Type.SpecialType != SpecialType.None
					? new BoundCopy(result.Syntax, variable)
					: new BoundMove(result.Syntax, variable);
			}
		}
		return result;
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
		TypeSymbol booleanType = GetSpecialType(SpecialType.StdBoolean);
		BoundExpression result = BindRValueWithoutTargetType(syntax, diagnostics);

		if (result.Type.SpecialType != SpecialType.StdBoolean)
		{
			diagnostics.Diagnostics.ReportCannotImplicitlyConvert(syntax.Location, result.Type, GetSpecialType(SpecialType.StdBoolean));
			return new BoundConversion(syntax, result, ConversionKind.NoConversion, booleanType, hasErrors: true);
		}

		return result;
	}

	private BoundExpression BindCastExpression(CastExpressionSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		BoundExpression operand = BindExpression(syntax.Left, diagnostics);
		TypeSymbol? targetType = null;
		if (syntax.TypeExpression is TypeSyntax typeSyntax)
		{
			targetType = BindType(typeSyntax, diagnostics);
		}

		if (targetType == null || targetType.IsErrorSymbol)
		{
			return operand;
		}

		ConversionKind kind = TypeConversions.ClassifyConversion(operand.Type, targetType);
		if (kind == ConversionKind.NoConversion)
		{
			diagnostics.Diagnostics.ReportCannotImplicitlyConvert(syntax.Location, operand.Type, targetType);
			return new BoundConversion(syntax, operand, ConversionKind.NoConversion, targetType, hasErrors: true);
		}

		return new BoundConversion(syntax, operand, kind, targetType);
	}

	internal static BoundConversion? ConvertImplicitly(BoundExpression expression, TypeSymbol targetType, BindingDiagnosticBag diagnostics)
	{
		ConversionKind kind = TypeConversions.ClassifyConversion(expression.Type, targetType);
		if (kind == ConversionKind.NoConversion)
		{
			return null;
		}

		if (!kind.IsImplicit)
		{
			return null;
		}

		BoundConversion conversion = new(expression.Syntax!, expression, kind, targetType);
		return conversion;
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
		NumberToken? valueToken = syntax.Token as NumberToken;
		Debug.Assert(valueToken != null);
		var value = valueToken.Value;

		unchecked
		{
			switch (valueToken.Type)
			{
				case NumericLiteralType.Any: // TODO: in future we possible want to lateinit theirs type for better resolution resolving
				{
					if (valueToken.Format != NumericLiteralFormat.Integer)
					{
						TypeSymbol f32 = GetSpecialType(SpecialType.StdNumericsFloat32);
						return new BoundLiteral(syntax, ConstantValue.Create((float)value.F64), f32);
					}
					else
					{
						TypeSymbol i32 = GetSpecialType(SpecialType.StdNumericsSInt32);
						return new BoundLiteral(syntax, ConstantValue.Create((int)value.U64), i32);
					}
				}
				case NumericLiteralType.I8:
				{
					TypeSymbol type = GetSpecialType(SpecialType.StdNumericsSInt8);
					return new BoundLiteral(syntax, ConstantValue.Create((int)(sbyte)value.U64), type);
				}
				case NumericLiteralType.I16:
				{
					TypeSymbol type = GetSpecialType(SpecialType.StdNumericsSInt16);
					return new BoundLiteral(syntax, ConstantValue.Create((short)value.U64), type);
				}
				case NumericLiteralType.I32:
				{
					TypeSymbol type = GetSpecialType(SpecialType.StdNumericsSInt32);
					return new BoundLiteral(syntax, ConstantValue.Create((int)value.U64), type);
				}
				case NumericLiteralType.I64:
				{
					TypeSymbol type = GetSpecialType(SpecialType.StdNumericsSInt64);
					return new BoundLiteral(syntax, ConstantValue.Create(value.U64), type);
				}
				case NumericLiteralType.U8:
				{
					TypeSymbol type = GetSpecialType(SpecialType.StdNumericsUInt8);
					return new BoundLiteral(syntax, ConstantValue.Create((int)(byte)value.U64), type);
				}
				case NumericLiteralType.U16:
				{
					TypeSymbol type = GetSpecialType(SpecialType.StdNumericsUInt16);
					return new BoundLiteral(syntax, ConstantValue.Create((ushort)value.U64), type);
				}
				case NumericLiteralType.U32:
				{
					TypeSymbol type = GetSpecialType(SpecialType.StdNumericsUInt32);
					return new BoundLiteral(syntax, ConstantValue.Create((uint)value.U64), type);
				}
				case NumericLiteralType.U64:
				{
					TypeSymbol type = GetSpecialType(SpecialType.StdNumericsUInt64);
					return new BoundLiteral(syntax, ConstantValue.Create(value.U64), type);
				}
				case NumericLiteralType.Signed:
				{
					TypeSymbol type = GetSpecialType(SpecialType.StdNumericsSInt32);
					return new BoundLiteral(syntax, ConstantValue.Create((int)value.U64), type);
				}
				case NumericLiteralType.Unsigned:
				{
					TypeSymbol type = GetSpecialType(SpecialType.StdNumericsUInt32);
					return new BoundLiteral(syntax, ConstantValue.Create((int)(uint)value.U64), type);
				}
				case NumericLiteralType.F16:
				{
					TypeSymbol type = GetSpecialType(SpecialType.StdNumericsFloat16);
					return new BoundLiteral(syntax, ConstantValue.Create((float)value.F64), type);
				}
				case NumericLiteralType.F32:
				{
					TypeSymbol type = GetSpecialType(SpecialType.StdNumericsFloat32);
					return new BoundLiteral(syntax, ConstantValue.Create((float)value.F64), type);
				}
				case NumericLiteralType.F64:
				{
					TypeSymbol type = GetSpecialType(SpecialType.StdNumericsFloat64);
					return new BoundLiteral(syntax, ConstantValue.Create((float)value.F64), type);
				}
				default:
					throw new UnreachableException();
			}
		}
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
		StringToken content = (syntax.Token as StringToken)!;
		SpecialType sliceType = content.LiteralType switch
		{
			StringLiteralType.Unicode16 => SpecialType.StdTextStringSliceUtf16,
			StringLiteralType.Unicode32 => SpecialType.StdTextStringSliceUtf32,
			_ => SpecialType.StdTextStringSliceUtf8
		};

		TypeSymbol strType = GetSpecialType(sliceType);
		TypeSymbol refType = Compilation.CreateReferenceType(strType, isMutable: false, isNullable: false, Compilation.GetStaticLifetime());

		return new BoundLiteral(syntax, ConstantValue.Create(content.Text), refType);
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

				var candidates = new FunctionSymbol[group.Count];
				for (int i = 0; i < group.Count; i++)
					candidates[i] = (FunctionSymbol)group[i];

				var receiver = SynthesizeFunctionGroupReceiver(group);

				boundExpression = new BoundFunctionGroup(name, [..candidates], receiver, result.Kind, CreateErrorType());
			}
			else if (symbol is NamedTypeSymbol typeSymbol && invoked)
			{
				// Treat invoked type name as constructor call
				ImmutableArray<Symbol> members = typeSymbol.GetMembers();
				var constructors = ArrayBuilder<FunctionSymbol>.GetInstance();
				foreach (Symbol member in members)
				{
					if (member is ConstructorSymbol ctor)
					{
						constructors.Add(ctor);
					}
				}

				if (constructors.Count > 0)
				{
					boundExpression = new BoundFunctionGroup(name, constructors.ToImmutableAndFree(),
						receiver: null, result.Kind, CreateErrorType());
				}
				else
				{
					constructors.Free();
					boundExpression = BindNonFunction(name, symbol, diagnostics, result.Kind, indexed, isError);
				}
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

	private BoundExpression BindNonFunction(SyntaxNode syntax, Symbol symbol, BindingDiagnosticBag diagnostics,
		LookupResultKind resultKind, bool indexed, bool wasError)
	{
		switch (symbol.Kind)
		{
			case SymbolKind.LocalVariable:
			{
				var local = (LocalVariableSymbol)symbol;
				return new BoundLocalVariable(syntax, local);
			}
			case SymbolKind.Parameter:
			{
				var param = (ParameterSymbol)symbol;
				return new BoundParameter(syntax, param);
			}
			case SymbolKind.NamedType:
			case SymbolKind.GenericTypeParameter:
				return new BoundTypeExpression(syntax, (TypeSymbol)symbol, hasErrors: wasError);
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

	private BoundExpression? SynthesizeFunctionGroupReceiver(ArrayBuilder<Symbol> members)
	{
		Debug.Assert(members.Count > 0);

		TypeSymbol? currentType = ContainingType;
		if (currentType == null)
		{
			return null;
		}

		var declaringType = members[0].ContainingType;

		// if (currentType.IsEqualToOrDerivedFrom(declaringType, TypeCompareKind.ConsiderEverything))
		// {
		// 	return new (syntax, currentType, wasCompilerGenerated: true);
		// }

		return null;
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
