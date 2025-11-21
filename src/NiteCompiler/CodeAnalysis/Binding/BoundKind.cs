namespace NiteCompiler.CodeAnalysis.Binding;

internal enum BoundKind
{
	CompilationUnit,
	Type,
	Field,

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