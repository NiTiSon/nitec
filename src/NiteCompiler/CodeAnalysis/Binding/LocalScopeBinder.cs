using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Symbols;

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
}