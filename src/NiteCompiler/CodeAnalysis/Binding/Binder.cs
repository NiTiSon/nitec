using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Compilation;

namespace NiteCompiler.CodeAnalysis.Binding;

internal abstract class Binder
{
	public NiteCompilation NiteCompilation { get; }
	protected Binder? Parent { get; }

	protected Binder(NiteCompilation niteCompilation, Binder? parent)
	{
		NiteCompilation = niteCompilation;
		Parent = parent;
	}

	protected abstract Symbol? LookupSymbol(string name, LookupOptions options = LookupOptions.Default);
	protected Symbol? LookupSymbolInParent(string name, LookupOptions options = LookupOptions.Default)
		=> Parent?.LookupSymbol(name, options);

	public abstract BoundNode Bind(SyntaxNode syntax);
}