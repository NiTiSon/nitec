using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class InFunctionBinder : Binder
{
	private Dictionary<string, ParameterSymbol>? _lateinitParameterMap;
	private readonly FunctionSymbol _owner;

	public InFunctionBinder(FunctionSymbol owner, Binder parent) : base(parent)
	{
		_owner = owner;
	}

	public override Symbol ContainingMember => _owner;

	internal override void LookupSymbolsInSingleBinder(LookupResult result, string name, int arity, LookupOptions options,
		Binder originalBinder, bool diagnose)
	{
		Debug.Assert(result.IsClear);

		if (_owner.Parameters.Length == 0)
		{
			return;
		}

		var parameterMap = _lateinitParameterMap;
		if (parameterMap == null)
		{
			ImmutableArray<ParameterSymbol> parameters = _owner.Parameters;
			parameterMap = new Dictionary<string, ParameterSymbol>(parameters.Length, EqualityComparer<string>.Default);
			foreach (var parameter in parameters)
			{

				parameterMap.Add(parameter.Name, parameter);
			}

			_lateinitParameterMap = parameterMap;
		}

		if (parameterMap.TryGetValue(name, out var parameterSymbol))
		{
			result.MergeEqual(originalBinder.CheckViability(parameterSymbol, arity, options, null, diagnose));
		}
	}
}