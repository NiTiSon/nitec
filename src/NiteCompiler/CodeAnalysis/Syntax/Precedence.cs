namespace NiteCompiler.CodeAnalysis.Syntax;

public enum Precedence : uint
{
	Expression = 0,
	Assignment = Expression,
	ConditionalOr,
	ConditionalAnd,
	LogicalOr,
	LogicalXor,
	LogicalAnd,
	Equality,
	Relational,
	Shift,
	Additive,
	Multiplicative,
	Switch,
	Range,
	Unary,
	Cast,
	PointerIndirection,
	AddressOf,
	Primary
}