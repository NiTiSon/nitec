namespace NiteCompiler.CodeAnalysis.Binding;

internal enum BoundKind
{
	None = 0,
	Move = 1,
	Copy = 2,
	Block,
	Return,
	ExpressionStatement,
	IfStatement,
	LoopStatement,
	WhileStatement,
	ForStatement,
	DoWhileStatement,

	FunctionBody,
	VariableDeclaration,

	Literal,
	Local,
	Parameter,
	UnaryExpression,
	BinaryExpression,
	CompoundAssignmentExpression,
	VariableExpression,
	AssignmentExpression,
	BadExpression
}