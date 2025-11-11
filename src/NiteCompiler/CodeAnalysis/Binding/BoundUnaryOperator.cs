using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundUnaryOperator
{
	public SyntaxKind SyntaxKind { get; }
	public BoundUnaryOperatorKind Kind { get; }
	public TypeSymbol ValueType { get; }
	public TypeSymbol Type { get; }

	private BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind kind, TypeSymbol type)
		: this(syntaxKind, kind, type, type, type)
	{
	}

	private BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind kind, TypeSymbol operandType, TypeSymbol resultType)
		: this(syntaxKind, kind, operandType, operandType, resultType)
	{
	}

	private BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind kind, TypeSymbol valueType, TypeSymbol rightType, TypeSymbol resultType)
	{
		SyntaxKind = syntaxKind;
		Kind = kind;
		ValueType = valueType;
		Type = resultType;
	}
}