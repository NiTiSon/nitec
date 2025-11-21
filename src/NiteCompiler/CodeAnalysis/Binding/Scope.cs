using System.Runtime.InteropServices;
using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class Scope
{
	private readonly Scope? _parent;

	public Scope([Optional] Scope? parent)
	{
		_parent = parent;
	}

	public Symbol? Lookup(string name, LookupOptions options = LookupOptions.Default)
	{
		return _parent?.Lookup(name, options);
	}

	// public LocalSymbol Declare(ParameterSymbol parameter)
	// {
	//
	// }
}