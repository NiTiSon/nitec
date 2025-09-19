using System;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

internal static class SyntaxFacts
{
	public static ReadOnlySpan<SyntaxKind> AccessKeywords =>
	[
		SyntaxKind.PublicKeyword, SyntaxKind.ProtectedKeyword, SyntaxKind.InternalKeyword,
		SyntaxKind.PrivateKeyword, SyntaxKind.FamilyKeyword, SyntaxKind.FriendKeyword,
	];

	public static bool IsPossibleKeyword(int lexemeWidth)
	{
		return lexemeWidth is >= 2 and <= 11;
		// as, is - keywords 2 chars in wide
		// commutative - keyword 11 chars in wide
		// all that outside [2..11] characters wide definitely not a keyword
	}

	public static void DefineKeywordOrIdentifier(string identifier, ref NiteLexer.TokenInfo info)
	{
		switch (identifier)
		{
			case "use":
				info.Kind = SyntaxKind.UseKeyword;
				return;
			case "module":
				info.Kind = SyntaxKind.ModuleKeyword;
				return;
			case "public":
				info.Kind = SyntaxKind.PublicKeyword;
				return;
			case "private":
				info.Kind = SyntaxKind.PrivateKeyword;
				return;
			case "protected":
				info.Kind = SyntaxKind.ProtectedKeyword;
				return;
			case "friend":
				info.Kind = SyntaxKind.FriendKeyword;
				return;
			case "family":
				info.Kind = SyntaxKind.FamilyKeyword;
				return;
			case "internal":
				info.Kind = SyntaxKind.InternalKeyword;
				return;
			case "let":
				info.Kind = SyntaxKind.LetKeyword;
				return;
			case "return":
				info.Kind = SyntaxKind.ReturnKeyword;
				return;
			case "i8":
				info.Kind = SyntaxKind.I8Keyword;
				return;
			case "i16":
				info.Kind = SyntaxKind.I16Keyword;
				return;
			case "i32":
				info.Kind = SyntaxKind.I32Keyword;
				return;
			case "i64":
				info.Kind = SyntaxKind.I64Keyword;
				return;
			case "u8":
				info.Kind = SyntaxKind.U8Keyword;
				return;
			case "u16":
				info.Kind = SyntaxKind.U16Keyword;
				return;
			case "u32":
				info.Kind = SyntaxKind.U32Keyword;
				return;
			case "u64":
				info.Kind = SyntaxKind.U64Keyword;
				return;
			case "f16":
				info.Kind = SyntaxKind.F16Keyword;
				return;
			case "f32":
				info.Kind = SyntaxKind.F32Keyword;
				return;
			case "f64":
				info.Kind = SyntaxKind.F64Keyword;
				return;
			case "void":
				info.Kind = SyntaxKind.VoidKeyword;
				return;
			case "bool":
				info.Kind = SyntaxKind.BoolKeyword;
				return;
		}
	}

	public static int GetLineBreakWidth(SourceText text, int i)
	{
		char c = text[i];
		char next = i + 1 < text.Length ? text[i + 1] : '\0';

		if (c == '\r' && next == '\n') return 2;
		if (c == '\r' || c == '\n') return 1;

		return 0;
	}

	public static bool IsTypeKeyword(SyntaxKind kind)
	{
		return kind switch
		{
			SyntaxKind.I8Keyword or
			SyntaxKind.I16Keyword or
			SyntaxKind.I32Keyword or
			SyntaxKind.I64Keyword or
			SyntaxKind.U8Keyword or
			SyntaxKind.U16Keyword or
			SyntaxKind.U32Keyword or
			SyntaxKind.U64Keyword or
			SyntaxKind.F16Keyword or
			SyntaxKind.F32Keyword or
			SyntaxKind.F64Keyword or
			SyntaxKind.VoidKeyword or
			SyntaxKind.BoolKeyword => true,
			_ => false
		};
	}
}