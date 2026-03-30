using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal partial class Binder
{
	private void LookupIdentifier(LookupResult result, SimpleNameSyntax node, bool invoked)
	{
		LookupIdentifier(result, name: node.GetName(), 0/*node.Arity*/, invoked);
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
}