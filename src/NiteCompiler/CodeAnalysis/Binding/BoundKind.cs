namespace NiteCompiler.CodeAnalysis.Binding;

internal enum BoundKind
{
	CompilationUnit,
	TypeDeclaration,
	FunctionDeclaration,

	BlockStatement,
	ExpressionStatement,
	ReturnStatement,

	ErrorExpression,
	LiteralExpression,
	VariableExpression,
	AssignmentExpression,
	UnaryExpression,
	BinaryExpression,
}