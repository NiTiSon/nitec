namespace NiteCompiler.CodeAnalysis.Binding;

internal enum BoundUnaryOperatorKind
{
	Plus,
	Minus,
	LogicalNot,
	BitwiseNot,
	PointerIndirection,
	AddressOf,
}