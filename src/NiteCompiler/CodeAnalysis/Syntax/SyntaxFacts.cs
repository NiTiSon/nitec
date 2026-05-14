using System;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

internal static class SyntaxFacts
{
	public const char DigitDelimiter = '\'';

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
			case "i8":
				info.Kind = TokenKind.I8;
				return;
			case "i16":
				info.Kind = TokenKind.I16;
				return;
			case "i32":
				info.Kind = TokenKind.I32;
				return;
			case "i64":
				info.Kind = TokenKind.I64;
				return;
			case "u8":
				info.Kind = TokenKind.U8;
				return;
			case "u16":
				info.Kind = TokenKind.U16;
				return;
			case "u32":
				info.Kind = TokenKind.U32;
				return;
			case "u64":
				info.Kind = TokenKind.U64;
				return;
			case "f16":
				info.Kind = TokenKind.F16;
				return;
			case "f32":
				info.Kind = TokenKind.F32;
				return;
			case "f64":
				info.Kind = TokenKind.F64;
				return;
			case "void":
				info.Kind = TokenKind.Void;
				return;
			case "bool":
				info.Kind = TokenKind.Boolean;
				return;
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
				info.Kind = TokenKind.Public.ToContextualKeyword();
				return;
			case "friend":
				info.Kind = TokenKind.Friend.ToContextualKeyword();
				return;
			case "protected":
				info.Kind = TokenKind.Protected.ToContextualKeyword();
				return;
			case "internal":
				info.Kind = TokenKind.Internal.ToContextualKeyword();
				return;
			case "family":
				info.Kind = TokenKind.Family.ToContextualKeyword();
				return;
			case "private":
				info.Kind = TokenKind.Private.ToContextualKeyword();
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
			case "interface":
				info.Kind = TokenKind.Interface;
				return;
			case "where":
				info.Kind = TokenKind.Where.ToContextualKeyword();
				return;
			case "partial":
				info.Kind = TokenKind.Partial;
				return;
			case "unsized":
				info.Kind = TokenKind.Unsized;
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

	public static bool TryGetEscapedCharacter(ref char escapedCharacter)
	{
		switch (escapedCharacter)
		{
			case '\'':
				escapedCharacter = '\'';
				return true;
			case '\"':
				escapedCharacter = '\"';
				return true;
			case '`':
				escapedCharacter = '`';
				return true;
			case '\\':
				escapedCharacter = '\\';
				return true;
			case 'n':
				escapedCharacter = '\n';
				return true;
			case 'r':
				escapedCharacter = '\r';
				return true;
			case 't':
				escapedCharacter = '\t';
				return true;
			case 'b':
				escapedCharacter = '\b';
				return true;
			case 'v':
				escapedCharacter = '\v';
				return true;
			case '0':
				escapedCharacter = '\x00';
				return true;
			default:
				return false;
		}
	}
}