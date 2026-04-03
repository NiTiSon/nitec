namespace NiteCompiler.CodeAnalysis.Binding.Operators;

internal enum UnaryOperatorKind
{
	Error = 0,
	Plus, // +x
	Negate, // -x
	BitwiseNot, // ~x
	LogicalNot, // !x
	Circumflex, // ^x
}

internal static class UnaryOperatorKindExtensions
{
	extension(UnaryOperatorKind kind)
	{
		public int ToIndex() => (int)kind - 1;
	}
}