using System.Collections.Frozen;
using System.Linq;
using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public static class SyntaxFacts
{
	private static readonly string[] Keywords;
	private static readonly FrozenDictionary<string, SyntaxKind> KeywordFrozen;
	
	public static string? GetText(SyntaxKind kind)
	{
		if ((uint)kind is >= 8192 and < 32768)
		{
			int offset = (int)kind - 8192;
		}

		return null;
	}

	public static int GetUnaryPrecedence(SyntaxKind kind)
	{
		switch (kind)
		{
			case SyntaxKind.PlusToken:
			case SyntaxKind.MinusToken:
				return 21;

			default:
				return 0;
		}
	}

	public static bool IsTypeKeyword(SyntaxKind kind)
	{
		return kind is >= SyntaxKind.I8Keyword and <= SyntaxKind.BoolKeyword;
	}

	public static int GetPrecedence(SyntaxKind kind)
	{
		switch (kind)
		{
			case SyntaxKind.AsteriskToken: // *
			case SyntaxKind.SlashToken: // / (divide)
			case SyntaxKind.PercentToken: // % (modulo)
				return 20;

			case SyntaxKind.PlusToken: // +
			case SyntaxKind.MinusToken: // -
				return 19;
			
			case SyntaxKind.LessThanLessThanToken: // <<
			case SyntaxKind.GreaterThanGreaterThanToken: // >>
			case SyntaxKind.GreaterThanGreaterThanGreaterThanToken: // >>>
				return 18;

			case SyntaxKind.DotDotToken: // ..
			case SyntaxKind.DotDotEqualsToken: // ..=
				return 15;

			case SyntaxKind.LessThanToken: // <=
			case SyntaxKind.LessThanEqualsToken: // <
			case SyntaxKind.GreaterThanToken: // >
			case SyntaxKind.GreaterThanEqualsToken: // >=
			case SyntaxKind.AsKeyword: // as
			case SyntaxKind.IsKeyword: // is
				return 10;

			case SyntaxKind.EqualsEqualsToken: // ==
			case SyntaxKind.ExclamationMarkEqualsToken: // !=
				return 9;

			case SyntaxKind.AmpersandToken:
				return 7;
			case SyntaxKind.CircumflexToken:
				return 6;
			case SyntaxKind.PipeToken:
				return 5;

			case SyntaxKind.AmpersandAmpersandToken:
				return 4;
			case SyntaxKind.PipePipeToken:
				return 3;

			default:
				return 0;
		}
	}

	public static SyntaxKind? GetKind(string text)
	{
		if (KeywordFrozen.TryGetValue(text, out SyntaxKind kind))
		{
			return kind;
		}

		return null;
	}

	static SyntaxFacts()
	{
		Keywords =
		[
			"true",
			"false",
			"use",
			"if",
			"else",
			"loop",
			"for",
			"do",
			"while",
			"typeof",
			"sizeof",
			"offsetof",
			"default",
			"break",
			"return",
			"continue",
			"where",
			"when",
			"is",
			"as",
			"const",
			"public",
			"friend",
			"protected",
			"internal",
			"family",
			"private",
			"virtual",
			"static",
			"volatile",
			"get",
			"set",
			"operator",
			"u8",
			"u16",
			"u32",
			"u64",
			"i8",
			"i16",
			"i32",
			"i64",
			"f16",
			"f32",
			"f64",
			"void",
			"bool",
			"module",
			"type",
		];

		SyntaxKind kind = (SyntaxKind)8191;
		KeywordFrozen = Keywords.Select(t =>
		{
			kind++;
			return new {t, kind};
		}).ToDictionary(t => t.t, t => t.kind).ToFrozenDictionary();
	}
}