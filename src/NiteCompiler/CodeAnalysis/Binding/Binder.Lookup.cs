using System;
using System.Collections.Immutable;
using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Symbols.Source;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal partial class Binder
{
	/// <summary>
	/// Performs name lookup for simple generic or non-generic name
	/// within an optional qualifier namespace or type symbol.
	/// If LookupOption.AttributeTypeOnly is set, then it performs
	/// attribute type lookup which involves attribute name lookup
	/// with and without "Attribute" suffix.
	/// </summary>
	internal void LookupSymbolsSimpleName(LookupResult result, ContainerSymbol? qualifier, string plainName, int arity,
		LookupOptions options, bool diagnose)
	{
		// if (options.IsAttributeTypeLookup())
		// {
		// 	LookupAttributeType(result, qualifierOpt, plainName, arity, basesBeingResolved, options, diagnose, ref useSiteInfo);
		// }
		// else
		// {
		LookupSymbolsOrMembersInternal(result, qualifier, plainName, arity, options, diagnose);
		// }
	}

	private void LookupSymbolsOrMembersInternal(LookupResult result, ContainerSymbol? qualifier, string name, int arity,
		LookupOptions options,bool diagnose)
	{
		if (qualifier == null)
		{
			LookupSymbolsInternal(result, name, arity, options, diagnose);
		}
		else
		{
			LookupMembersInternal(result, qualifier, name, arity, options, this, diagnose);
		}
	}

	protected void LookupMembersInternal(LookupResult result, ContainerSymbol qualifier, string name, int arity,
		LookupOptions options, Binder originalBinder, bool diagnose)
	{
		Debug.Assert(arity >= 0);
		if (qualifier.Kind == SymbolKind.Module)
		{
			LookupMembersInModule(result, (ModuleSymbol)qualifier, name, arity, options, originalBinder, diagnose);
		}
		else
		{
			LookupMembersInType(result, (TypeSymbol)qualifier, name, arity, options, originalBinder, diagnose);
		}
	}

	protected void LookupMembersInType(LookupResult result, TypeSymbol type, string name, int arity,
		LookupOptions options, Binder originalBinder, bool diagnose)
	{
		switch (type.TypeKind)
		{
			case TypeKind.TypeParameter:
				throw new NotImplementedException();

			case TypeKind.SimpleType:
				LookupMembersInBasicType(result, type, name, arity, options, originalBinder, diagnose);
				break;
		}
	}

	protected static void LookupMembersWithoutInheritance(LookupResult result, TypeSymbol type, string name, int arity,
		LookupOptions options, Binder originalBinder, TypeSymbol accessThroughType, bool diagnose)
	{
		var members = GetCandidateMembers(type, name, options, originalBinder);

		foreach (Symbol member in members)
		{
			SingleLookupResult resultOfThisMember = originalBinder.CheckViability(member, arity, options, accessThroughType, diagnose);
			result.MergeEqual(resultOfThisMember);
		}
	}

	private void LookupMembersInBasicType(LookupResult result, TypeSymbol type, string name, int arity,
		LookupOptions options, Binder originalBinder, bool diagnose)
	{
		LookupMembersInBasicType(result, type, name, arity, options, originalBinder, type, diagnose);
	}

	private void LookupMembersInBasicType(LookupResult result, TypeSymbol type, string name, int arity,
		LookupOptions options, Binder originalBinder, TypeSymbol accessThroughType, bool diagnose)
	{
		Debug.Assert(type.TypeKind != TypeKind.TypeParameter);

		LookupMembersWithoutInheritance(result, type, name, arity, options, originalBinder, accessThroughType,
			diagnose);
	}

	private static void LookupMembersInModule(LookupResult result, ModuleSymbol module, string name, int arity,
		LookupOptions options, Binder originalBinder, bool diagnose)
	{
		var members = GetCandidateMembers(module, name, options, originalBinder);

		foreach (Symbol member in members)
		{
			SingleLookupResult resultOfThisMember = originalBinder.CheckViability(member, arity, options, null, diagnose);
			result.MergeEqual(resultOfThisMember);
		}
	}

	internal SingleLookupResult CheckViability(Symbol symbol, int arity, LookupOptions options, TypeSymbol? accessThroughType, bool diagnose)
	{
		if (options.HasFlag(LookupOptions.ModulesOrTypesOnly) && symbol is not (ModuleSymbol or TypeSymbol))
		{
			return new SingleLookupResult(LookupResultKind.NotAModuleNorAType, symbol, null);
		}

		if (options.HasFlag(LookupOptions.AttributesOnly) && symbol is not AttributeSymbol)
		{
			return new SingleLookupResult(LookupResultKind.NotAnAttribute, symbol, null);
		}

		// TODO: Check WrongArity when symbol has type parameters and arity doesn't match
		// TODO: Check Inaccessible when symbol is not accessible from current context
		// TODO: Check NotInvocable when MustBeInvocableIfMember is set and symbol is not invocable
		// TODO: Check NotCreatable when type cannot be instantiated

		return LookupResult.Viable(symbol);
	}

	private void LookupIdentifier(LookupResult result, SimpleNameSyntax node, bool invoked)
	{
		LookupIdentifier(result, name: node.GetName(), node.Arity, invoked);
	}

	private void LookupIdentifier(LookupResult result, string name, int arity, bool invoked)
	{
		LookupOptions options = LookupOptions.IgnoreFunctionArity;
		if (invoked)
		{
			options |= LookupOptions.MustBeInvocableIfMember;
		}

		// if (!IsInMethodBody && !IsInsideNameof)
		// {
		// 	Debug.Assert((options & LookupOptions.ModulesOrTypesOnly) == 0);
		// 	options |= LookupOptions.MustNotBeMethodTypeParameter;
		// }

		LookupSymbolsWithFallback(result, name, arity, options: options);
	}

	private void LookupAttribute(LookupResult result, string name, int arity)
	{
		LookupOptions options = LookupOptions.AttributesOnly;

		LookupSymbolsWithFallback(result, name, arity, options: options);
	}

	internal virtual SourceLocalVariableSymbol LookupLocalVariable(SimpleNameSyntax nameSyntax)
	{
		Debug.Assert(Parent != null);
		return Parent.LookupLocalVariable(nameSyntax);
	}

	internal FieldSymbol? LookupFieldSymbolWithinType(TypeSymbol type, string name)
	{
		ImmutableArray<Symbol> candidates = GetCandidateMembers(type, name, LookupOptions.Default, this);

		if (candidates is [FieldSymbol field])
		{
			return field;
		}

		// TODO: probably may cause problems when several members with same name are presented
		return null;
	}

	private Binder LookupSymbolsWithFallback(LookupResult result, string name, int arity, LookupOptions options)
	{
		Binder binder = LookupSymbolsInternal(result, name, arity, options, diagnose: false);
		Debug.Assert((binder != null) || result.IsClear);

		if (result.Kind != LookupResultKind.Viable && result.Kind != LookupResultKind.Empty)
		{
			result.Clear();
			// retry to get diagnosis
			var otherBinder = LookupSymbolsInternal(result, name, arity, options, diagnose: true);
			Debug.Assert(binder == otherBinder);
		}

		Debug.Assert(result.IsMultiViable || result.IsClear || result.Error != null);
		return binder;
	}

	private Binder LookupSymbolsInternal(LookupResult result, string name, int arity,
		LookupOptions options, bool diagnose)
	{
		Debug.Assert(result.IsClear);

		Binder? binder = null;
		for (var scope = this; scope != null && !result.IsMultiViable; scope = scope.Parent)
		{
			if (binder != null)
			{
				var tmp = LookupResult.GetInstance();
				scope.LookupSymbolsInSingleBinder(tmp, name, arity, options, this, diagnose);
				result.MergeEqual(tmp);
				tmp.Free();
			}
			else
			{
				scope.LookupSymbolsInSingleBinder(result, name, arity, options, this, diagnose);
				if (!result.IsClear)
				{
					binder = scope;
				}
			}
		}
		return binder;
	}

	internal virtual void LookupSymbolsInSingleBinder(LookupResult result, string name, int arity, LookupOptions options,
		Binder originalBinder, bool diagnose)
	{
	}

	internal static ImmutableArray<Symbol> GetCandidateMembers(ContainerSymbol moduleOrType, string name,
		LookupOptions options, Binder originalBinder)
	{
		if ((options & LookupOptions.ModulesOrTypesOnly) != 0 && moduleOrType is TypeSymbol)
		{
			return moduleOrType.GetTypeMembers(name).CastArray<Symbol>();
		}
		else
		{
			return moduleOrType.GetMembers(name);
		}
	}
}