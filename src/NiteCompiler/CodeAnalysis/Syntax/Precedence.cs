namespace NiteCompiler.CodeAnalysis.Syntax;

public enum Precedence : byte
{
	Expression = 0,
	Assignment = Expression,
	Ternary,
	ConditionalOr,
	ConditionalAnd,
	BitwiseOr, // bitwise or aka logical or
	BitwiseXor,
	BitwiseAnd,
	Equality,
	Relational,
	Shift,
	Additive,
	Multiplicative,
	Range,
	Unary,
	Cast,
	Primary
}