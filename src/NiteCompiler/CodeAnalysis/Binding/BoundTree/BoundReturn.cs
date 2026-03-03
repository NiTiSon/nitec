using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding.BoundTree;

internal sealed class BoundReturn : BoundStatement
{
	public BoundExpression Expression { get; }
	public override BoundKind Kind => BoundKind.Return;

	public BoundReturn(SyntaxNode syntax, BoundExpression expression) : base(syntax)
	{
		Expression = expression;
	}
}