namespace NiteCompiler.CodeAnalysis.Binding;

internal abstract class BoundVisitor
{
	public virtual void Visit(BoundNode? node)
	{
		node?.Accept(this);
	}

	protected virtual void DefaultVisit(BoundNode node) { }

	public virtual void VisitBadExpression(BoundBadExpression badExpression) => DefaultVisit(badExpression);
	public virtual void VisitAssignment(BoundAssignment assignment) => DefaultVisit(assignment);
	public virtual void VisitBinaryExpression(BoundBinaryExpression binaryExpression) => DefaultVisit(binaryExpression);
	public virtual void VisitBlock(BoundBlock block) => DefaultVisit(block);
	public virtual void VisitCopy(BoundCopy copy) => DefaultVisit(copy);
	public virtual void VisitExpressionStatement(BoundExpressionStatement statement) => DefaultVisit(statement);
	public virtual void VisitFunctionBody(BoundFunctionBody functionBody) => DefaultVisit(functionBody);
	public virtual void VisitIfStatement(BoundIfStatement ifStatement) => DefaultVisit(ifStatement);
	public virtual void VisitLiteral(BoundLiteral literal) => DefaultVisit(literal);
	public virtual void VisitLocal(BoundLocal local) => DefaultVisit(local);
	public virtual void VisitMove(BoundMove move) => DefaultVisit(move);
	public virtual void VisitReturn(BoundReturn returnStatement) => DefaultVisit(returnStatement);
}