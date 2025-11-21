using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal abstract class Binder
{
	public Compilation Compilation { get; }
	protected Binder? Parent { get; }

	protected Binder(Compilation compilation, Binder? parent)
	{
		Compilation = compilation;
		Parent = parent;
	}

	protected abstract Symbol? LookupSymbol(string name, LookupOptions options = LookupOptions.Default);
	protected Symbol? LookupSymbolInParent(string name, LookupOptions options = LookupOptions.Default)
		=> Parent?.LookupSymbol(name, options);

	public abstract BoundNode Bind(SyntaxNode syntax);
}