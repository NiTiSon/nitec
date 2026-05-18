namespace NiteCompiler.CodeAnalysis.Binding;

internal enum BoundKind
{
	None = 0,
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
	Parameter,
	LocalVariable,
	Move,
	Copy,
	AddressOfExpression,
	DereferenceExpression,
	UnaryExpression,
	BinaryExpression,
	AssignmentExpression,
	CompoundAssignmentExpression,
	InvocationExpression,
	IndexationExpression,
	VariableExpression,
	TypeExpression,
	FieldAccess,
	FunctionGroup,
	BadExpression
}