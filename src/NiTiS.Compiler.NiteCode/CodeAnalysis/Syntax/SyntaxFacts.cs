using System.Runtime.CompilerServices;

namespace NiTiS.Compiler.NiteCode.CodeAnalysis.Syntax;

public static class SyntaxFacts
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsIdentifierBeginCharacter(char c, ref bool notAKeyword)
	{
		return char.IsAsciiLetter(c) // Fast path
		       || (notAKeyword = c == '_') // If c is '_' -> identifier isn't a keyword
		       || char.IsLetter(c); // Slower path
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsIdentifierContinueCharacter(char c)
	{
		return char.IsAsciiLetterOrDigit(c)
		       || c == '_'
		       || char.IsLetterOrDigit(c);
	}

	public static int GetOperatorPrecedence(SyntaxKind kind)
	{
		return kind switch
		{
			SyntaxKind.DotDotToken => 1, // Ranges
			SyntaxKind.DotDotEqualsToken => 1,
			SyntaxKind.PlusToken => 10, // Addition
			SyntaxKind.MinusToken => 10,
			SyntaxKind.AsteriskToken => 11, // Multiplication
			SyntaxKind.SlashToken => 11,
			SyntaxKind.PercentToken => 11,

			SyntaxKind.PlusPlusToken => 19, // Postfix inc/dec
			SyntaxKind.MinusMinusToken => 19,
			SyntaxKind.DotToken => 20, // Member access
			_ => 0,
		};
	}

	public static string? GetText(SyntaxKind kind)
	{
		return kind switch
		{
			SyntaxKind.PlusToken => "+",
			SyntaxKind.MinusToken => "-",
			SyntaxKind.ExclamationToken => "!",
			SyntaxKind.DoubleQuote => "\"",
			SyntaxKind.HashToken => "#",
			SyntaxKind.DollarToken => "$",
			SyntaxKind.PercentToken => "%",
			SyntaxKind.AmpersandToken => "&",
			SyntaxKind.QuoteToken => "'",
			SyntaxKind.OpenParenToken => "(",
			SyntaxKind.CloseParenToken => ")",
			SyntaxKind.AsteriskToken => "*",
			SyntaxKind.CommaToken => ",",
			SyntaxKind.DotToken => ".",
			SyntaxKind.SlashToken => "/",
			SyntaxKind.ColonToken => ":",
			SyntaxKind.SemicolonToken => ";",
			SyntaxKind.LessThanToken => "<",
			SyntaxKind.EqualsToken => "=",
			SyntaxKind.GreaterThanToken => ">",
			SyntaxKind.QuestionToken => "?",
			SyntaxKind.AtToken => "@",
			SyntaxKind.OpenBracketToken => "[",
			SyntaxKind.BackslashToken => "\\",
			SyntaxKind.CloseBracketToken => "]",
			SyntaxKind.CircumflexToken => "^",
			SyntaxKind.UnderscoreToken => "_",
			//SyntaxKind.BacktickToken => "`",
			SyntaxKind.OpenBraceToken => "{",
			SyntaxKind.PipeToken => "|",
			SyntaxKind.CloseBraceToken => "}",
			SyntaxKind.TildeToken => "~",
			SyntaxKind.MinusMinusToken => "--",
			SyntaxKind.PlusPlusToken => "++",
			SyntaxKind.PlusEqualsToken => "+=",
			SyntaxKind.MinusEqualsToken => "-=",
			SyntaxKind.AsteriskEqualsToken => "*=",
			SyntaxKind.SlashEqualsToken => "/=",
			SyntaxKind.PercentEqualsToken => "%=",
			SyntaxKind.AmpersandEqualsToken => "&=",
			SyntaxKind.PipeEqualsToken => "|=",
			SyntaxKind.CircumflexEqualsToken => "^=",
			SyntaxKind.LeftShiftToken => "<<",
			SyntaxKind.RightShiftToken => ">>",
			SyntaxKind.UnsignedRightShiftToken => ">>>",
			SyntaxKind.LeftShiftEqualsToken => "<<=",
			SyntaxKind.RightShiftEqualsToken => ">>=",
			SyntaxKind.UnsignedRightShiftEqualsToken => ">>>=",
			SyntaxKind.EqualsEqualsToken => "==",
			SyntaxKind.RetusaToken => "->",
			SyntaxKind.ColonColonToken => "::",
			SyntaxKind.DotDotToken => "..",
			SyntaxKind.DotDotEqualsToken => "..=",

			// Keywords
			SyntaxKind.TrueKeyword => "true",
			SyntaxKind.FalseKeyword => "false",
			SyntaxKind.UseKeyword => "use",
			SyntaxKind.IfKeyword => "if",
			SyntaxKind.ElseKeyword => "else",
			SyntaxKind.LoopKeyword => "loop",
			SyntaxKind.ForKeyword => "for",
			SyntaxKind.DoKeyword => "do",
			SyntaxKind.WhileKeyword => "while",
			SyntaxKind.BreakKeyword => "break",
			SyntaxKind.ReturnKeyword => "return",
			SyntaxKind.ContinueKeyword => "continue",
			SyntaxKind.GetKeyword => "get",
			SyntaxKind.SetKeyword => "set",
			SyntaxKind.WhereKeyword => "where",
			SyntaxKind.WhenKeyword => "when",
			SyntaxKind.IsKeyword => "is",
			SyntaxKind.AsKeyword => "as",
			SyntaxKind.OperatorKeyword => "operator",
			SyntaxKind.CommutativeKeyword => "commutative", // Honestly, NiteCode must be the only language with this keyword
			SyntaxKind.PublicKeyword => "public",
			SyntaxKind.FriendKeyword => "friend",
			SyntaxKind.ProtectedKeyword => "protected",
			SyntaxKind.InternalKeyword => "internal",
			SyntaxKind.FamilyKeyword => "family",
			SyntaxKind.PrivateKeyword => "private",

			SyntaxKind.I8Keyword => "i8",
			SyntaxKind.I16Keyword => "i16",
			SyntaxKind.I32Keyword => "i32",
			SyntaxKind.I64Keyword => "i64",
			SyntaxKind.U8Keyword => "u8",
			SyntaxKind.U16Keyword => "u16",
			SyntaxKind.U32Keyword => "u32",
			SyntaxKind.U64Keyword => "u64",
			SyntaxKind.F16Keyword => "f16",
			SyntaxKind.F32Keyword => "f32",
			SyntaxKind.F64Keyword => "f64",
			SyntaxKind.VoidKeyword => "void",
			SyntaxKind.BoolKeyword => "bool",
			_ => null
		};
	}

	public static SyntaxKind GetKind(string text)
	{
		// Probably changing == with other comparison will improve performance
		// Compared are always same size, same first letter (not always), not null
		if (text.Length is < 2 or > 11)
		{
			return SyntaxKind.Identifier;
		}

		switch (text.Length)
		{
			case 2 when text == "i8":
				return SyntaxKind.I8Keyword;
			case 2 when text == "u8":
				return SyntaxKind.U8Keyword;
			case 2 when text == "is":
				return SyntaxKind.IsKeyword;
			case 2 when text == "if":
				return SyntaxKind.IfKeyword;
			case 2 when text == "as":
				return SyntaxKind.AsKeyword;
			case 2 when text == "do":
				return SyntaxKind.DoKeyword;
			case 3:
				switch (text[0])
				{
					case 'g' when text == "get":
						return SyntaxKind.GetKeyword;
					case 's' when text == "set":
						return SyntaxKind.SetKeyword;
					case 'f':
						switch (text)
						{
							case "for":
								return SyntaxKind.ForKeyword;
							case "f32":
								return SyntaxKind.F32Keyword;
							case "f64":
								return SyntaxKind.F64Keyword;
							case "f16":
								return SyntaxKind.F16Keyword;
						}
						break;
					case 'i':
						switch (text)
						{
							case "i32": return SyntaxKind.I32Keyword;
							case "i64": return SyntaxKind.I64Keyword;
							case "i16": return SyntaxKind.I16Keyword;
						}
						break;
					case 'u':
						switch (text)
						{
							case "u32": return SyntaxKind.U32Keyword;
							case "use": return SyntaxKind.UseKeyword;
							case "u64": return SyntaxKind.U64Keyword;
							case "u16": return SyntaxKind.U16Keyword;
						}
						break;
				}
				break;
			case 4:
				switch (text[0])
				{
					case 't' when text == "true":
						return SyntaxKind.TrueKeyword;
					case 'e' when text == "else":
						return SyntaxKind.ElseKeyword;
					case 'l' when text == "loop":
						return SyntaxKind.LoopKeyword;
					case 'w' when text == "when":
						return SyntaxKind.WhenKeyword;
					case 'v' when text == "void":
						return SyntaxKind.VoidKeyword;
					case 'b' when text == "bool":
						return SyntaxKind.BoolKeyword;
				}
				break;
			case 5:
				switch (text[0])
				{
					case 'f' when text == "false":
						return SyntaxKind.FalseKeyword;
					case 'w':
						switch (text)
						{
							case "where": return SyntaxKind.WhereKeyword;
							case "while": return SyntaxKind.WhileKeyword;
						}
						break;
					case 'b' when text == "break":
						return SyntaxKind.BreakKeyword;
				}
				break;
			case 6:
				switch (text[0])
				{
					case 'r' when text == "return":
						return SyntaxKind.ReturnKeyword;
					case 'p' when text == "public":
						return SyntaxKind.PublicKeyword;
					case 'f':
						switch (text)
						{
							case "friend": return SyntaxKind.FriendKeyword;
							case "family": return SyntaxKind.FamilyKeyword;
						}
						break;
				}
				break;
			case 7 when text == "private":
				return  SyntaxKind.PrivateKeyword;
			case 8:
				switch (text[0])
				{
					case 'c' when text == "continue":
						return SyntaxKind.ContinueKeyword;
					case 'o' when text == "operator":
						return SyntaxKind.OperatorKeyword;
					case 'i' when text == "internal":
						return SyntaxKind.InternalKeyword;
				}
				break;
			case 9:
				switch (text[0])
				{
					case 'p' when text == "protected":
						return SyntaxKind.ProtectedKeyword;
				}
				break;
			case 11 when text == "commutative":
				return SyntaxKind.CommutativeKeyword;
		}

		return SyntaxKind.Identifier;
	}
}