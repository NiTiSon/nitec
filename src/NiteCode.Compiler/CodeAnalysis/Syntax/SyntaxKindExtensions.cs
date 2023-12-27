using System;
using System.Runtime.CompilerServices;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public static class SyntaxKindExtensions
{
	private const uint UnaryPrecedence = 6;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint GetUnaryOperatorPrecedence(this SyntaxKind kind)
	{
		return UnaryPrecedence;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint GetBinaryOperatorPrecedence(this SyntaxKind kind)
	{
		switch (kind)
		{
			// Divide and multiply operations are has highest priority
			case SyntaxKind.Slash:
			case SyntaxKind.Asterisk:
				return 5;

			// Then: basic ariθmetic operations
			case SyntaxKind.Plus:
			case SyntaxKind.Minus:
				return 4;

			// Comparison operations
			case SyntaxKind.EqualsEquals:
			case SyntaxKind.BangEquals:
			case SyntaxKind.Less:
			case SyntaxKind.LessEquals:
			case SyntaxKind.More:
			case SyntaxKind.MoreEquals:
				return 3;

			// AND, fast AND
			case SyntaxKind.Ampersand:
			case SyntaxKind.AmpersandAmpersand:
				return 2;

			// OR, fast OR, XOR
			case SyntaxKind.Bar:
			case SyntaxKind.BarBar:
			case SyntaxKind.Circumflex:
				return 1;

			default:
#if DEBUG
				Console.Error.WriteLine($"{nameof(SyntaxKindExtensions)}: Token '{kind}' has no precedence in binary operations.");
#endif
				return 0;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsOperator(this SyntaxKind kind)
		=> (kind & SyntaxKind.OperatorFlag) == SyntaxKind.OperatorFlag;

	public static bool IsFlag(this SyntaxKind kind)
		=> ((uint)kind | 0xFF000000) == 0xFF000000;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsKeyword(this SyntaxKind kind)
		=> (kind & SyntaxKind.KeywordFlag) == SyntaxKind.KeywordFlag;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsLiteralSymbol(this SyntaxKind kind)
		=> (kind & SyntaxKind.SymbolFlag) == SyntaxKind.SymbolFlag;

	public static bool IsBuiltInType(this SyntaxKind kind)
		=> (kind & SyntaxKind.BuiltInTypeFlag) == SyntaxKind.BuiltInTypeFlag;
}