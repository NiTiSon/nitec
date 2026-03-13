using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class InContainerBinder : Binder
{
	private ContainerSymbol _container;

	public override Symbol ContainingMember => _container;

	public InContainerBinder(ContainerSymbol container, Binder parent) : base(parent)
	{
		Debug.Assert(container != null);

		_container = container;
	}
}