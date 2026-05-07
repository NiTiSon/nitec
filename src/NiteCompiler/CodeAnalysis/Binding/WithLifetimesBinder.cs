using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.CodeAnalysis.Binding;

internal abstract class WithLifetimesBinder : Binder
{
	protected WithLifetimesBinder(Binder parent) : base(parent)
	{
	}

	protected abstract Dictionary<string, LifetimeSymbol> LifetimeMap { get; }

	internal override void LookupSymbolsInSingleBinder(LookupResult result, string name, int arity, LookupOptions options,
		Binder originalBinder, bool diagnose)
	{
		if ((options & LookupOptions.MustBeInvocableIfMember) != 0)
		{
			return;
		}

		if (LifetimeMap.TryGetValue(name, out var lifetimeSymbol))
		{
			result.MergeEqual(originalBinder.CheckViability(lifetimeSymbol, arity, options, null, diagnose));
		}
	}
}