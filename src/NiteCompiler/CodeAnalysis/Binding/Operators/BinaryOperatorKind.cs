using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding.Operators;

internal enum BinaryOperatorKind
{
	Error = 0,
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

internal static class BinaryOperatorKindExtensions
{
	extension(BinaryOperatorKind kind)
	{
		public int ToIndex() => (int)kind - 1;
	}
}