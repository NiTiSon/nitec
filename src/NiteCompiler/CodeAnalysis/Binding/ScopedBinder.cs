using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.Compilation;

namespace NiteCompiler.CodeAnalysis.Binding;

internal abstract class ScopedBinder : Binder
{
	protected Scope Scope { get; }

	protected ScopedBinder(NiteCompilation niteCompilation, Binder parent, Scope scope) : base(niteCompilation, parent)
	{
		Scope = scope;
	}

	protected override Symbol? LookupSymbol(string name, LookupOptions options = LookupOptions.Default)
	{
		Symbol? result = Scope.Lookup(name, options);

		return result ?? LookupSymbolInParent(name, options);
	}
}