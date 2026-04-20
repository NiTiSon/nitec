namespace NiteCompiler.CodeAnalysis.Binding;

internal enum BoundKind
{
	None = 0,
	Move,
	Copy,
	Block,
	EmptyStatement,
	ReturnStatement,
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