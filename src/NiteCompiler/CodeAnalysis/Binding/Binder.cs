using System.Collections.Immutable;
using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Compilation;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Binding;

internal abstract partial class Binder
{
	protected NiteCompilation Compilation { get; }

	public Binder? Parent { get; }
	public BinderFlags Flags { get; }

	public virtual Symbol? ContainingMember => null;

	protected Binder(NiteCompilation compilation)
	{
		Compilation = compilation;
	}

	protected Binder(Binder parent)
	{
		Compilation = parent.Compilation;
		Parent = parent;
	}

	protected Binder(Binder parent, BinderFlags flags)
	{
		Compilation = parent.Compilation;
		Parent = parent;
		Flags = flags;
	}

	public virtual ImmutableArray<LocalVariableSymbol> Locals => [];

	/// <summary>
	/// Some nodes have special binders for their contents (like Blocks)
	/// </summary>
	public virtual Binder? GetBinder(SyntaxNode node)
	{
		Debug.Assert(Parent != null);
		return Parent.GetBinder(node);
	}
}