using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal abstract class BoundNode
{
	protected BoundNode(SyntaxNode syntax)
	{
		Syntax = syntax;
	}

	public abstract BoundKind Kind { get; }
	public SyntaxNode Syntax { get; }
}