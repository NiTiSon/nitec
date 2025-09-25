using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal abstract class BoundExpression : BoundNode
{
	protected BoundExpression(SyntaxNode syntax)
		: base(syntax)
	{
	}

	public abstract TypeSymbol Type { get; }
	public virtual BoundConstant? ConstantValue => null;
}