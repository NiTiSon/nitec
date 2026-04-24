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
}