namespace NiteCompiler.CodeAnalysis.Binding.BoundTree;

internal enum BoundKind
{
	None = 0,
	Block,

	UnaryExpression,
	BinaryExpression,
}