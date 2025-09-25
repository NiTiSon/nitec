namespace NiteCompiler.CodeAnalysis.Binding;

internal enum BoundKind
{
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