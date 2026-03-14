using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding.BoundTree;

internal abstract class BoundExpression : BoundNode
{
	protected BoundExpression(SyntaxNode syntax) : base(syntax)
	{
	}

	public abstract TypeSymbol Type { get; }
	public abstract Binder.BindValueKind ValueKind { get; }
}