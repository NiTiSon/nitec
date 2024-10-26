using System;
using System.Globalization;

namespace Nlr.Compiler.CodeAnalysis.NiteCode;

public static class SyntaxFacts
{
	public static SyntaxKind GetKeywordKind(string text)
	{
		return text switch
		{
			"bool" => SyntaxKind.BoolKeyword,
			"u8" => SyntaxKind.U8Keyword,
			"u16" => SyntaxKind.U16Keyword,
			"u32" => SyntaxKind.U32Keyword,
			"u64" => SyntaxKind.U64Keyword,
			"i8" => SyntaxKind.I8Keyword,
			"i16" => SyntaxKind.I16Keyword,
			"i32" => SyntaxKind.I32Keyword,
			"i64" => SyntaxKind.I64Keyword,
			"f32" => SyntaxKind.F32Keyword,
			"f64" => SyntaxKind.F64Keyword,
			"void" => SyntaxKind.VoidKeyword,
			"typeof" => SyntaxKind.TypeOfKeyword,
			"sizeof" => SyntaxKind.SizeOfKeyword,
			"offsetof" => SyntaxKind.OffsetOfKeyword,
			"default" => SyntaxKind.DefaultKeyword,
			"nil" => SyntaxKind.NilKeyword,
			"true" => SyntaxKind.TrueKeyword,
			"false" => SyntaxKind.FalseKeyword,
			"use" => SyntaxKind.UseKeyword,
			"module" => SyntaxKind.ModuleKeyword,
			_ => SyntaxKind.None,
		};
	}

	public static bool IsContextualKeyword(SyntaxKind kind)
	{
		return false;
	}

	public static bool IsIdentifierBeginCharacter(char ch)
	{
		if (ch >= 'a' && ch <= 'z')
		{
			return true;
		}
		else if (ch >= 'A' && ch <= 'Z')
		{
			return true;
		}

		return IsBeginLetterCategory(CharUnicodeInfo.GetUnicodeCategory(ch));
	}

	public static bool IsIdentifierContinueCharacter(char ch)
	{
		if (ch >= 'a' && ch <= 'z')
		{
			return true;
		}
		else if (ch >= 'A' && ch <= 'Z')
		{
			return true;
		}

		return IsContinueCharCategory(CharUnicodeInfo.GetUnicodeCategory(ch));
	}

	private static bool IsBeginLetterCategory(UnicodeCategory category)
	{
		switch (category)
		{
			case UnicodeCategory.UppercaseLetter:
			case UnicodeCategory.LowercaseLetter:
			case UnicodeCategory.TitlecaseLetter:
			case UnicodeCategory.ModifierLetter:
			case UnicodeCategory.OtherLetter:
			case UnicodeCategory.LetterNumber:
				return true;
		}

		return false;
	}

	private static bool IsContinueCharCategory(UnicodeCategory category)
	{
		switch (category)
		{
			case UnicodeCategory.UppercaseLetter:
			case UnicodeCategory.LowercaseLetter:
			case UnicodeCategory.TitlecaseLetter:
			case UnicodeCategory.ModifierLetter:
			case UnicodeCategory.OtherLetter:
			case UnicodeCategory.LetterNumber:
			case UnicodeCategory.Format:
			case UnicodeCategory.ConnectorPunctuation:
			case UnicodeCategory.DecimalDigitNumber:
			case UnicodeCategory.SpacingCombiningMark:
			case UnicodeCategory.NonSpacingMark:
				return true;
		}

		return false;
	}

	public static bool IsWhitespace(char ch)
	{
		return (ch == ' ' || ch == '\t')
			|| CharUnicodeInfo.GetUnicodeCategory(ch) == UnicodeCategory.SpaceSeparator;
	}
}