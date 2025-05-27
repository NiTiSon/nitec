namespace Nlr.Compiler.NiteCode.CodeAnalysis;

public enum OperationKind
{
	None,
	Invalid = 0x01,
	Block = 0x02,
	VariableDeclaration = 0x03,
	Loop = 0x04,
	Empty = 0x05,
	Return = 0x06,
	Use = 0x07,
	ExpressionStatement = 0x08,
	Literal = 0x09,
	As = 0x0a,
	Is = 0x0b,
	Unary = 0x0c,
	Binary = 0x0d,
	Increment = 0x0e,
	Decrement = 0x0f,
	Argument = 0x10,
}