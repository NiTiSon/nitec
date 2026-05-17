using System;
using System.Diagnostics;
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
			NumericLiteralFormat.Float => ParseFloat(info, text, location, diagnostics),
			NumericLiteralFormat.ENotation => ParseENotation(info, text, location, diagnostics),
			_ => throw new UnreachableException()
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

	public static NumberToken.Packed ParseFloat(in NiteLexer.TokenInfo info, ReadOnlySpan<char> text, Location location, DiagnosticBag diagnostics)
	{
		return new NumberToken.Packed(ParseFloatToDouble(text));
	}

	public static NumberToken.Packed ParseENotation(in NiteLexer.TokenInfo info, ReadOnlySpan<char> text, Location location, DiagnosticBag diagnostics)
	{
		int eIndex = -1;
		for (int j = 0; j < text.Length; j++)
		{
			if (text[j] is 'e' or 'E')
			{
				eIndex = j;
				break;
			}
		}

		ReadOnlySpan<char> significandText = text[..eIndex];
		ReadOnlySpan<char> exponentText = text[(eIndex + 1)..];

		double significand = ParseFloatToDouble(significandText);

		int exponent = 0;
		int expIdx = 0;
		bool negativeExponent = false;

		if (expIdx < exponentText.Length && exponentText[expIdx] == '-')
		{
			negativeExponent = true;
			expIdx++;
		}
		else if (expIdx < exponentText.Length && exponentText[expIdx] == '+')
		{
			expIdx++;
		}

		while (expIdx < exponentText.Length && SyntaxFacts.IsValidDecimalDigit(exponentText[expIdx]))
		{
			if (exponentText[expIdx] == SyntaxFacts.DigitDelimiter)
			{
				expIdx++;
				continue;
			}
			exponent = exponent * 10 + (exponentText[expIdx] - '0');
			expIdx++;
		}

		if (negativeExponent)
			exponent = -exponent;

		double result = significand;
		if (exponent > 0)
		{
			for (int j = 0; j < exponent; j++)
				result *= 10;
		}
		else if (exponent < 0)
		{
			for (int j = 0; j < -exponent; j++)
				result /= 10;
		}

		return new NumberToken.Packed(result);
	}

	private static double ParseFloatToDouble(ReadOnlySpan<char> text)
	{
		double value = 0;
		int i = 0;

		while (i < text.Length && SyntaxFacts.IsValidDecimalDigit(text[i]))
		{
			if (text[i] == SyntaxFacts.DigitDelimiter)
			{
				i++;
				continue;
			}
			value = value * 10 + (text[i] - '0');
			i++;
		}

		if (i < text.Length && text[i] == '.')
		{
			i++;
			double fraction = 0;
			double divisor = 1;
			while (i < text.Length && SyntaxFacts.IsValidDecimalDigit(text[i]))
			{
				if (text[i] == SyntaxFacts.DigitDelimiter)
				{
					i++;
					continue;
				}
				fraction = fraction * 10 + (text[i] - '0');
				divisor *= 10;
				i++;
			}
			value += fraction / divisor;
		}

		return value;
	}
}