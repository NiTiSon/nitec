using System.Collections.Generic;
using System.Threading;
using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class WithFunctionGenericParametersBinder : Binder
{
	private readonly FunctionSymbol _function;
	private Dictionary<string, GenericTypeParameterSymbol>? _typeParamMap;

	public WithFunctionGenericParametersBinder(FunctionSymbol function, Binder parent) : base(parent)
	{
		_function = function;
	}

	private Dictionary<string, GenericTypeParameterSymbol> TypeParamMap
	{
		get
		{
			if (_typeParamMap == null)
			{
				Dictionary<string, GenericTypeParameterSymbol> result = [];
				foreach (var typeParam in _function.TypeParameters)
				{
					result.Add(typeParam.Name, typeParam);
				}
				Interlocked.CompareExchange(ref _typeParamMap, result, null);
			}
			return _typeParamMap;
		}
	}

	internal override void LookupSymbolsInSingleBinder(LookupResult result, string name, int arity,
		LookupOptions options, Binder originalBinder, bool diagnose)
	{
		if ((options & LookupOptions.MustBeInvocableIfMember) != 0) return;

		if (TypeParamMap.TryGetValue(name, out var typeParam))
		{
			result.MergeEqual(originalBinder.CheckViability(typeParam, arity, options, null, diagnose));
		}
	}
}
