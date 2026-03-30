using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding.BoundTree;

internal sealed class BoundIfStatement : BoundStatement
{
	public override BoundKind Kind => BoundKind.IfStatement;
	public BoundExpression Condition { get; }
	public BoundStatement ThenStatement { get; }
	public BoundStatement? ElseStatement { get; }

	public BoundIfStatement(SyntaxNode syntax, BoundExpression condition, BoundStatement thenStatement, BoundStatement? elseStatement) : base(syntax)
	{
		Condition = condition;
		ThenStatement = thenStatement;
		ElseStatement = elseStatement;
	}
}