using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class InContainerBinder : Binder
{
	public override ContainerSymbol ContainingMember { get; }

	public InContainerBinder(ContainerSymbol container, Binder parent) : base(parent)
	{
		Debug.Assert(container != null);

		ContainingMember = container;
	}

	internal override void LookupSymbolsInSingleBinder(LookupResult result, string name, int arity,
		LookupOptions options, Binder originalBinder, bool diagnose)
	{
		Debug.Assert(result.IsClear);

		foreach (Symbol member in ContainingMember.GetMembers())
		{
			if (member.Name != name)
			{
				continue;
			}

			result.MergeEqual(originalBinder.CheckViability(member, arity, options, null, diagnose));
		}
	}
}
