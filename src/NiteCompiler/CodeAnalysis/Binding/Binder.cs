using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Compilation;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Binding;

internal abstract class Binder
{
	protected NiteCompilation Compilation { get; }
	protected Binder? _parent;
	protected DiagnosticBag _diagnostics;

	protected Binder(NiteCompilation compilation)
	{
		Compilation = compilation;
		_diagnostics = [];
	}

	protected Binder(Binder parent)
	{
		Compilation = parent.Compilation;
		_parent = parent;
		_diagnostics = parent._diagnostics;
	}

	/// <summary>
	/// Some nodes have special binders for their contents (like Blocks)
	/// </summary>
	public virtual Binder? GetBinder(SyntaxNode node)
	{
		Debug.Assert(_parent != null);
		return _parent.GetBinder(node);
	}

	public abstract Symbol? Resolve();
}