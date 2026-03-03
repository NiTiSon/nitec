using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal enum BinaryOperatorKind
{
	Addition,
	Subtraction,
	Multiplication,
	Division,
	Modulo,
	LeftArithmeticShift,
	RightArithmeticShift,
	RightUnsignedShift,
	Equal,
	NotEqual,
	Greater,
	Less,
	GreaterOrEqual,
	LessOrEqual,
	And,
	Xor,
	Or,
	Tilde
}