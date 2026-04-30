using System;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.CodeAnalysis.Text;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis;

internal static class NumberParser
{
	public static NumberToken.Packed Parse(in NiteLexer.TokenInfo info, ReadOnlySpan<char> text, Location location, DiagnosticBag diagnostics)
	{
		return info.NumericFormat switch
		{
			NumericLiteralFormat.Integer => ParseInteger(info, text, location, diagnostics),
			_ => throw new NotImplementedException()
		};
	}

	public static NumberToken.Packed ParseInteger(in NiteLexer.TokenInfo info, ReadOnlySpan<char> text, Location location, DiagnosticBag diagnostics)
	{
		ulong value = 0;
		bool overflow = false;

		// x or b is only possible after 0; so no need to double-check
		if (text.Length >= 2 && text[1] is 'x' or 'b') text = text[2..];

		try
		{
			checked
			{
				foreach (char c in text)
				{
					if (c == SyntaxFacts.DigitDelimiter) continue;
					if (!SyntaxFacts.IsValidHexDigit(c)) break;

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
			diagnostics.ReportIntegralConstantIsTooLarge(location);
			return default;
		}
		else
		{
			return new(value);
		}
	}
}