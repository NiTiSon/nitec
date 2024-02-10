namespace NiteCode.Compiler.CodeAnalysis.Syntax;

internal static class SyntaxFacts
{
	public const uint UnaryPrecedence = 12;

	public static uint GetUnaryOperatorPrecedence(this SyntaxNode node)
		=> GetUnaryOperatorPrecedence(node.Kind);

	public static uint GetUnaryOperatorPrecedence(this SyntaxKind kind)
	{
		switch (kind)
		{
			case SyntaxKind.PlusToken:
			case SyntaxKind.MinusToken:
			case SyntaxKind.TildeToken:
			case SyntaxKind.AmpersandToken:
			case SyntaxKind.AsteriskToken:
			case SyntaxKind.ExclamationToken:
				return UnaryPrecedence;

			default: return 0;
		}
	}

	public static uint GetBinaryOperatorPrecedence(this SyntaxNode node)
		=> GetBinaryOperatorPrecedence(node.Kind);

	public static uint GetBinaryOperatorPrecedence(this SyntaxKind kind)
	{
		switch (kind)
		{
			//case SyntaxKind.RangeToken:
			//	return 11;

			case SyntaxKind.PercentToken:
			case SyntaxKind.AsteriskToken:
			case SyntaxKind.SlashToken:
				return 10;

			case SyntaxKind.PlusToken:
			case SyntaxKind.MinusToken:
				return 9;

			// Shifts

			//  Relational and type-testing

			// Binary and
			case SyntaxKind.AmpersandToken:
				return 6;

			// Binary xor
			case SyntaxKind.CircumflexToken:
				return 5;

			// Binary or
			case SyntaxKind.BarToken:
				return 4;

			// Conditional xor
			case SyntaxKind.AmpersandAmpersandToken:
				return 3;

			// Conditional or
			case SyntaxKind.BarBarToken:
				return 2;

			// Conditional expression

			// Not are binary operator
			default: return 0;
		}
	}
}