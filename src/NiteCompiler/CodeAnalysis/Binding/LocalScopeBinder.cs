using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Symbols.Source;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Binding;

internal class LocalScopeBinder : Binder
{
	public LocalScopeBinder(Binder parent) : base(parent)
	{
	}

	public LocalScopeBinder(Binder parent, BinderFlags flags) : base(parent, flags)
	{
	}

	public sealed override ImmutableArray<LocalVariableSymbol> Locals
	{
		get
		{
			if (field.IsDefault)
			{
				ImmutableInterlocked.InterlockedCompareExchange(ref field, BuildLocals(), default);
			}

			return field;
		}
	}
	private Dictionary<string, LocalVariableSymbol>? LocalsMap
	{
		get
		{
			if (field == null && Locals.Length > 0)
			{
				field = BuildMap(Locals);
			}

			return field;
		}
	}

	protected virtual ImmutableArray<LocalVariableSymbol> BuildLocals()
	{
		return [];
	}

	private static Dictionary<string, TSymbol> BuildMap<TSymbol>(ImmutableArray<TSymbol> array)
		where TSymbol : Symbol
	{
		Debug.Assert(array.Length > 0);

		Dictionary<string, TSymbol> map = [];

		for (int i = array.Length - 1; i >= 0; i--)
		{
			TSymbol symbol = array[i];
			map[symbol.Name] = symbol;
		}

		return map;
	}

	internal override SourceLocalVariableSymbol LookupLocalVariable(SimpleNameSyntax nameSyntax)
	{
		string identifier = nameSyntax.GetName();
		LocalVariableSymbol? result = null;
		if (LocalsMap != null && LocalsMap.TryGetValue(identifier, out result))
		{
			return (SourceLocalVariableSymbol)result;
		}

		return base.LookupLocalVariable(nameSyntax);
	}

	internal override void LookupSymbolsInSingleBinder(LookupResult result, string name, int arity, LookupOptions options,
		Binder originalBinder, bool diagnose)
	{
		Debug.Assert(result.IsClear);

		var localsMap = LocalsMap;
		if (localsMap != null)
		{
			if (localsMap.TryGetValue(name, out var localSymbol))
			{
				result.MergeEqual(originalBinder.CheckViability(localSymbol, arity, options, null, diagnose));
			}
		}
	}

	protected ImmutableArray<LocalVariableSymbol> BuildLocals(Binder enclosingBinder, SyntaxList<StatementSyntax> statements)
	{
#if DEBUG
		Binder currentBinder = enclosingBinder;

		while (true)
		{
			if (this == currentBinder)
			{
				break;
			}

			currentBinder = currentBinder.Parent!;
		}
#endif
		var builder = ArrayBuilder<LocalVariableSymbol>.GetInstance();

		foreach (StatementSyntax statement in statements)
		{
			BuildLocals(enclosingBinder, statement, builder);
		}

		return builder.ToImmutableAndFree();
	}

	private void BuildLocals(Binder enclosingBinder, StatementSyntax statement, ArrayBuilder<LocalVariableSymbol> locals)
	{
		if (statement.Kind == NodeKind.LocalVariableDeclarationStatement)
		{
			Binder localDeclarationBinder = enclosingBinder.GetBinder(statement) ?? enclosingBinder;
			var declaration = (LocalVariableDeclarationStatement)statement;

			var localSymbol = MakeLocalVariable(declaration, declaration.Declarator, localDeclarationBinder);
			locals.Add(localSymbol);
		}
	}

	private LocalVariableSymbol MakeLocalVariable(LocalVariableDeclarationStatement syntax,
		LocalVariableDeclarator declarator, Binder? initializerBinder = null)
	{
		Debug.Assert(Parent != null);

		string name = syntax.Declarator.Name.GetName();
		Location nameLocation = syntax.Declarator.Name.Location;
		SyntaxReference reference = syntax.Declarator.CreateReference();

		return new SourceLocalVariableSymbol(ContainingMember, this, declarator.TypeClause,
			declarator.EqualsValueClause, initializerBinder, isAssignable: true, name, nameLocation, reference);
	}
}