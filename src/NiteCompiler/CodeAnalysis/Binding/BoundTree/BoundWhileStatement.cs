using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundWhileStatement : BoundStatement
{
	public BoundExpression Condition { get; }
	public BoundStatement Body { get; }

	public override BoundKind Kind => BoundKind.WhileStatement;

	public BoundWhileStatement(SyntaxNode? syntax, BoundExpression condition, BoundStatement body,  bool hasErrors = false) : base(syntax, hasErrors)
	{
		Condition = condition;
		Body = body;
	}

	public override void Accept(BoundVisitor visitor)
	{
		visitor.VisitWhileStatement(this);
	}
}