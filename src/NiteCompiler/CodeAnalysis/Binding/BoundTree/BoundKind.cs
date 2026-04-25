namespace NiteCompiler.CodeAnalysis.Binding;

internal enum BoundKind
{
	None = 0,
	Move,
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
	VariableDeclarationStatement,

	Literal,
	Local,
	Parameter,
	UnaryExpression,
	BinaryExpression,
	AssignmentExpression,
	CompoundAssignmentExpression,
	InvocationExpression,
	IndexationExpression,
	VariableExpression,
	BadExpression
}