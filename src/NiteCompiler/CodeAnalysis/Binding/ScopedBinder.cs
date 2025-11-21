using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.CodeAnalysis.Binding;

internal abstract class ScopedBinder : Binder
{
	protected Scope Scope { get; }

	protected ScopedBinder(Compilation compilation, Binder parent, Scope scope) : base(compilation, parent)
	{
		Scope = scope;
	}

	protected override Symbol? LookupSymbol(string name, LookupOptions options = LookupOptions.Default)
	{
		Symbol? result = Scope.Lookup(name, options);

		return result ?? LookupSymbolInParent(name, options);
	}
}