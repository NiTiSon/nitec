using NiteCompiler.CodeAnalysis.Binding.Pure;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal abstract class BoundExpression : BoundNode
{
	protected BoundExpression(SyntaxNode syntax) : base(syntax)
	{
	}

	protected BoundExpression(SyntaxNode syntax, bool hasErrors) : base(syntax, hasErrors)
	{
	}

	public abstract TypeSymbol Type { get; }
	public abstract Pureness Pureness { get; }
	public abstract Binder.BindValueKind ValueKind { get; }
}