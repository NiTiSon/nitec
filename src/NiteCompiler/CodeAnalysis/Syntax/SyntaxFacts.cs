using System;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

internal static class SyntaxFacts
{
	public const char DigitDelimiter = '\'';

	public static ReadOnlySpan<SyntaxKind> AccessKeywords =>
	[
		SyntaxKind.PublicKeyword, SyntaxKind.ProtectedKeyword, SyntaxKind.InternalKeyword,
		SyntaxKind.PrivateKeyword, SyntaxKind.FamilyKeyword, SyntaxKind.FriendKeyword,
	];

	public static ReadOnlySpan<SyntaxKind> ModifierKeywords =>
	[
		SyntaxKind.StaticKeyword,
		SyntaxKind.ConstKeyword,
		SyntaxKind.VirtualKeyword, SyntaxKind.OverrideKeyword, SyntaxKind.AbstractKeyword, SyntaxKind.SealedKeyword,
	];

	public static bool IsPossibleKeyword(int lexemeWidth)
	{
		return lexemeWidth is >= 2 and <= 11;
		// as, is - keywords 2 chars in wide
		// commutative - keyword 11 chars in wide
		// all that outside [2..11] characters wide is definitely not a keyword
	}

	public static void DefineKeywordOrIdentifier(string identifier, ref NiteLexer.TokenInfo info)
	{
	    switch (identifier)
	    {
	        case "true":
	            info.Kind = TokenKind.True;
	            return;
	        case "false":
	            info.Kind = TokenKind.False;
	            return;
	        case "use":
	            info.Kind = TokenKind.Use;
	            return;
	        case "if":
	            info.Kind = TokenKind.If;
	            return;
	        case "else":
	            info.Kind = TokenKind.Else;
	            return;
	        case "loop":
	            info.Kind = TokenKind.Loop;
	            return;
	        case "while":
	            info.Kind = TokenKind.While;
	            return;
	        case "for":
	            info.Kind = TokenKind.For;
	            return;
	        case "do":
	            info.Kind = TokenKind.Do;
	            return;
	        case "module":
	            info.Kind = TokenKind.Module;
	            return;
	        case "type":
	            info.Kind = TokenKind.Type;
	            return;
	        case "break":
	            info.Kind = TokenKind.Break;
	            return;
	        case "return":
	            info.Kind = TokenKind.Return;
	            return;
	        case "public":
	            info.Kind = TokenKind.Public;
	            return;
	        case "friend":
	            info.Kind = TokenKind.Friend;
	            return;
	        case "protected":
	            info.Kind = TokenKind.Protected;
	            return;
	        case "internal":
	            info.Kind = TokenKind.Internal;
	            return;
	        case "family":
	            info.Kind = TokenKind.Family;
	            return;
	        case "private":
	            info.Kind = TokenKind.Private;
	            return;
	        case "let":
	            info.Kind = TokenKind.Let;
	            return;
	        case "static":
	            info.Kind = TokenKind.Static;
	            return;
	        case "const":
	            info.Kind = TokenKind.Const;
	            return;
	        case "pure":
	            info.Kind = TokenKind.Pure;
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

	public static bool IsValidHexDigit(char number)
	{
		return number
			is >= '0' and <= '9'
			or >= 'A' and <= 'F'
			or >= 'a' and <= 'f'
			or DigitDelimiter;
	}

	public static bool IsValidDecimalDigit(char number)
	{
		return number
			is >= '0' and <= '9'
			or DigitDelimiter;
	}

	public static bool IsValidBinaryDigit(char number)
	{
		return number
			is '0'
			or '1'
			or DigitDelimiter;
	}
}