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
	public virtual void VisitCompoundAssignment(BoundCompoundAssignment assignment) => DefaultVisit(assignment);
	public virtual void VisitUnaryExpression(BoundUnaryExpression unaryExpression) => DefaultVisit(unaryExpression);
	public virtual void VisitBinaryExpression(BoundBinaryExpression binaryExpression) => DefaultVisit(binaryExpression);
	public virtual void VisitCall(BoundCall call) => DefaultVisit(call);
	//public virtual void VisitIndexation(BoundIndexation indexation) => DefaultVisit(indexation);
	public virtual void VisitBlock(BoundBlock block) => DefaultVisit(block);
	public virtual void VisitMove(BoundMove move) => DefaultVisit(move);
	public virtual void VisitEmptyStatement(BoundEmptyStatement statement) => DefaultVisit(statement);
	public virtual void VisitExpressionStatement(BoundExpressionStatement statement) => DefaultVisit(statement);
	public virtual void VisitFunctionBody(BoundFunctionBody functionBody) => DefaultVisit(functionBody);
	public virtual void VisitLocalVariableDeclarationStatement(BoundLocalVariableDeclarationStatement declaration) => DefaultVisit(declaration);
	public virtual void VisitIfStatement(BoundIfStatement ifStatement) => DefaultVisit(ifStatement);
	public virtual void VisitLoopStatement(BoundLoopStatement loopStatement) => DefaultVisit(loopStatement);
	public virtual void VisitWhileStatement(BoundWhileStatement whileStatement) => DefaultVisit(whileStatement);
	public virtual void VisitLiteral(BoundLiteral literal) => DefaultVisit(literal);
	public virtual void VisitLocal(BoundLocal local) => DefaultVisit(local);
	public virtual void VisitParameter(BoundParameter parameter) => DefaultVisit(parameter);
	public virtual void VisitReturn(BoundReturn returnStatement) => DefaultVisit(returnStatement);
}