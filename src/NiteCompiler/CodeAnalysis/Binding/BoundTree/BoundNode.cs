using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal abstract class BoundNode
{
	public abstract BoundKind Kind { get; }
	public SyntaxNode Syntax { get; }
	public bool HasErrors { get; }

	protected BoundNode(SyntaxNode syntax, bool hasErrors = false)
	{
		Syntax = syntax;
		HasErrors = hasErrors;
	}

	public abstract void Accept(BoundVisitor visitor);
}