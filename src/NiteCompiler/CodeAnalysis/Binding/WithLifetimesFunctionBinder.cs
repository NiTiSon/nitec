using System.Collections.Generic;
using System.Threading;
using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class WithLifetimesFunctionBinder : WithLifetimesBinder
{
	public override FunctionSymbol ContainingMember { get; }

	public WithLifetimesFunctionBinder(FunctionSymbol functionSymbol, Binder parent) : base(parent)
	{
		ContainingMember = functionSymbol;
	}

	protected override Dictionary<string, LifetimeSymbol> LifetimeMap
	{
		get
		{
			if (field == null)
			{
				Dictionary<string, LifetimeSymbol> result = [];
				foreach (var typeParameter in ContainingMember.Lifetimes)
				{
					result.Add(typeParameter.Name, typeParameter);
				}

				Interlocked.CompareExchange(ref field, result, null);
			}

			return field;
		}
	}
}