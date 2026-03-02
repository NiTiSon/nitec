using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding.BoundTree;

internal abstract class BoundNode
{
	public abstract BoundKind Kind { get; }
	public SyntaxNode Syntax { get; }

	protected BoundNode(SyntaxNode syntax)
	{
		Syntax = syntax;
	}
}