using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

partial class Lexer
{
	private static readonly FrozenDictionary<SyntaxKind, StringSegment> literals;
	private static readonly FrozenDictionary<StringSegment, SyntaxKind> keywords;
	static Lexer()
	{
		Dictionary<SyntaxKind, StringSegment> literals = new()
		{
			[SyntaxKind.NilConst] = "nil",
			[SyntaxKind.TrueConst] = "true",
			[SyntaxKind.FalseConst] = "false",

			[SyntaxKind.Semicolon] = ";",
			[SyntaxKind.Colon] = ":",
			[SyntaxKind.Dot] = ".",
			[SyntaxKind.Comma] = ",",

			[SyntaxKind.OpenBrace] = "{",
			[SyntaxKind.CloseBrace] = "}",
			[SyntaxKind.OpenBracket] = "[",
			[SyntaxKind.CloseBracket] = "]",
			[SyntaxKind.OpenParenthesis] = "(",
			[SyntaxKind.CloseParenthesis] = ")",

			[SyntaxKind.Plus] = "+",
			[SyntaxKind.PlusPlus] = "++",
			[SyntaxKind.PlusEquals] = "+=",
			[SyntaxKind.Minus] = "-",
			[SyntaxKind.MinusMinus] = "--",
			[SyntaxKind.MinusEquals] = "-=",
			[SyntaxKind.Asterisk] = "*",
			[SyntaxKind.AsteriskEquals] = "*=",
			[SyntaxKind.Slash] = "/",
			[SyntaxKind.SlashEquals] = "/=",
			[SyntaxKind.Modulo] = "%",
			[SyntaxKind.ModuloEquals] = "%=",

			[SyntaxKind.Bang] = "!",
			[SyntaxKind.Equals] = "=",
			[SyntaxKind.EqualsEquals] = "==",
			[SyntaxKind.BangEquals] = "!=",
			[SyntaxKind.Tilde] = "~",
			[SyntaxKind.Circumflex] = "^",
			[SyntaxKind.CircumflexEquals] = "^=",
			[SyntaxKind.Ampersand] = "&",
			[SyntaxKind.AmpersandAmpersand] = "&&",
			[SyntaxKind.AmpersandEquals] = "&=",
			[SyntaxKind.Bar] = "|",
			[SyntaxKind.BarBar] = "||",
			[SyntaxKind.BarEquals] = "|=",
			[SyntaxKind.Less] = "<",
			[SyntaxKind.LessLess] = "<<",
			[SyntaxKind.LessEquals] = "<=",
			[SyntaxKind.LessLessEquals] = "<<=",
			[SyntaxKind.More] = ">",
			[SyntaxKind.MoreMore] = ">>",
			[SyntaxKind.MoreEquals] = ">=",
			[SyntaxKind.MoreMoreEquals] = ">>=",

			[SyntaxKind.I8] = "i8",
			[SyntaxKind.I16] = "i16",
			[SyntaxKind.I32] = "i32",
			[SyntaxKind.I64] = "i64",
			[SyntaxKind.I128] = "i128",
			[SyntaxKind.U8] = "u8",
			[SyntaxKind.U16] = "u16",
			[SyntaxKind.U32] = "u32",
			[SyntaxKind.U64] = "u64",
			[SyntaxKind.U128] = "u128",
			[SyntaxKind.F16] = "f16",
			[SyntaxKind.F32] = "f32",
			[SyntaxKind.F64] = "f64",
			[SyntaxKind.F128] = "f128",
			[SyntaxKind.Bool] = "bool",
			[SyntaxKind.Void] = "void",

			[SyntaxKind.PublicKeyword] = "public",
			[SyntaxKind.PrivateKeyword] = "private",
			[SyntaxKind.ProtectedKeyword] = "protected",
			[SyntaxKind.NewKeyword] = "new",
			[SyntaxKind.DeleteKeyword] = "delete",
			[SyntaxKind.UsingKeyword] = "using",
			[SyntaxKind.ModuleKeyword] = "module",
			[SyntaxKind.TypeKeyword] = "type",
			[SyntaxKind.EnumKeyword] = "enum",
			[SyntaxKind.RefKeyword] = "ref",
			[SyntaxKind.ObjectKeyword] = "object",
			[SyntaxKind.LocalKeyword] = "local",
			[SyntaxKind.ConstKeyword] = "const",
			[SyntaxKind.LiteralKeyword] = "literal",
			[SyntaxKind.StaticKeyword] = "static",
			[SyntaxKind.SizeofKeyword] = "sizeof",
			[SyntaxKind.TypeofKeyword] = "typeof",
			[SyntaxKind.DefaultKeyword] = "default",
			[SyntaxKind.OperatorKeyword] = "operator",
			[SyntaxKind.ExternalKeyword] = "external",
			[SyntaxKind.ImplicitKeyword] = "implicit",

			[SyntaxKind.IfKeyword] = "if",
			[SyntaxKind.ElseKeyword] = "else",
			[SyntaxKind.WhileKeyword] = "while",
			[SyntaxKind.DoKeyword] = "do",
			[SyntaxKind.ReturnKeyword] = "return",
		};
		Dictionary<StringSegment, SyntaxKind> keywords = [];

		foreach (SyntaxKind kind in Enum.GetValues<SyntaxKind>())
		{
			if ((kind.IsKeyword() || kind.IsBuiltInType()) && !kind.IsFlag())
				keywords[literals[kind]] = kind;
		}

		Lexer.literals = literals.ToFrozenDictionary();
		Lexer.keywords = keywords.ToFrozenDictionary();
	}
}