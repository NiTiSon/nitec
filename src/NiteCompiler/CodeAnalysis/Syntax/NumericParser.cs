using System;
using NiteCompiler.CodeAnalysis.Text;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Syntax;

internal static class NumericParser
{
	public static Token Parse(ref NiteLexer.TokenInfo info, ReadOnlySpan<char> text, SourceSpan span, DiagnosticBag diagnostics)
	{
		// TODO: Add suffixes logic
		// By default, all literals are signed
		ulong value = 0;
		bool overflow = false;

		try
		{
			checked
			{
				foreach (char c in text)
				{
					value *= 10;
					value += (ulong)(c - '0');
				}
			}
		}
		catch (OverflowException)
		{
			overflow = true;
		}

		if (overflow)
		{
			diagnostics.ReportIntegralConstantIsTooLarge(span);
			return new TokenWithValue<int>(SyntaxKind.NumberToken, span, 0, PredefinedType.I32);
		}

		if (value <= int.MaxValue)
		{
			return new TokenWithValue<int>(SyntaxKind.NumberToken, span, (int)value, PredefinedType.I32);
		}
		else if (value <= long.MaxValue)
		{
			return new TokenWithValue<long>(SyntaxKind.NumberToken, span, (long)value, PredefinedType.I64);
		}
		else
		{
			return new TokenWithValue<ulong>(SyntaxKind.NumberToken, span, value, PredefinedType.U64);
		}
	}
}