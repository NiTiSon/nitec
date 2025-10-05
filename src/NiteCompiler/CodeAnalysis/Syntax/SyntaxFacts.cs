using System;
using System.ComponentModel;
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

	public static bool IsLiteralExpression(SyntaxKind token)
	{
		return GetLiteralExpression(token) != SyntaxKind.None;
	}

	public static SyntaxKind GetLiteralExpression(SyntaxKind token)
	{
		return token switch
		{
			SyntaxKind.NumberToken => SyntaxKind.NumericLiteralExpression,
			SyntaxKind.TrueKeyword => SyntaxKind.TrueLiteralExpression,
			SyntaxKind.FalseKeyword => SyntaxKind.FalseLiteralExpression,
			_ => SyntaxKind.None,
		};
	}

	public static bool IsBinaryExpressionOperatorToken(SyntaxKind kind)
	{
		return GetBinaryExpression(kind) != SyntaxKind.None;
	}

	public static SyntaxKind GetBinaryExpression(SyntaxKind kind)
	{
		return kind switch
		{
			SyntaxKind.PlusToken => SyntaxKind.AddExpression,
			SyntaxKind.MinusToken => SyntaxKind.SubtractExpression,
			SyntaxKind.AsteriskToken => SyntaxKind.MultiplyExpression,
			SyntaxKind.SlashToken => SyntaxKind.DivideExpression,
			SyntaxKind.PercentToken => SyntaxKind.ModuloExpression,
			SyntaxKind.EqualsEqualsToken => SyntaxKind.EqualsExpression,
			SyntaxKind.ExclamationEqualsToken => SyntaxKind.NotEqualsExpression,
			_ => SyntaxKind.None,
		};
	}

	public static bool IsAssignmentExpressionOperatorToken(SyntaxKind kind)
	{
		return GetAssignmentExpression(kind) != SyntaxKind.None;
	}

	public static SyntaxKind GetAssignmentExpression(SyntaxKind kind)
	{
		return kind switch
		{
			SyntaxKind.EqualsToken => SyntaxKind.AssignmentExpression,
			SyntaxKind.PlusEqualsToken => SyntaxKind.AddAssignmentExpression,
			SyntaxKind.MinusEqualsToken => SyntaxKind.SubtractAssignmentExpression,
			SyntaxKind.AsteriskEqualsToken => SyntaxKind.MultiplyAssignmentExpression,
			SyntaxKind.SlashEqualsToken => SyntaxKind.DivideAssignmentExpression,
			SyntaxKind.PercentEqualsToken => SyntaxKind.ModuloAssignmentExpression,
			SyntaxKind.AmpersandEqualsToken => SyntaxKind.AndAssignmentExpression,
			SyntaxKind.CaretEqualsToken => SyntaxKind.XorAssignmentExpression,
			SyntaxKind.PipeEqualsToken =>  SyntaxKind.XorAssignmentExpression,
			_ =>  SyntaxKind.None,
		};
	}

	public static bool IsRightAssociativeExpression(SyntaxKind kind)
	{
		return kind
			is SyntaxKind.AssignmentExpression
			or SyntaxKind.AddAssignmentExpression
			or SyntaxKind.SubtractAssignmentExpression
			or SyntaxKind.MultiplyAssignmentExpression
			or SyntaxKind.DivideAssignmentExpression
			or SyntaxKind.ModuloAssignmentExpression
			or SyntaxKind.AndAssignmentExpression
			or SyntaxKind.XorAssignmentExpression
			or SyntaxKind.OrAssignmentExpression
			or SyntaxKind.CoalesceExpression;
	}

	public static bool IsUnaryExpression(SyntaxKind kind)
	{
		return GetUnaryExpression(kind) != SyntaxKind.None;
	}

	public static SyntaxKind GetUnaryExpression(SyntaxKind kind)
	{
		return kind switch
		{
			SyntaxKind.PlusToken => SyntaxKind.UnaryAddExpression,
			SyntaxKind.MinusToken => SyntaxKind.UnarySubtractExpression,
			SyntaxKind.ExclamationToken => SyntaxKind.UnaryLogicalNotExpression,
			SyntaxKind.TildeToken => SyntaxKind.UnaryBitwiseNotExpression,
			SyntaxKind.AmpersandToken => SyntaxKind.UnaryAddressOfExpression,
			SyntaxKind.AsteriskToken => SyntaxKind.UnaryPointerIndirectionExpression,
			_ => SyntaxKind.None
		};
	}

	public static Precedence GetPrecedence(SyntaxKind opKind)
	{
		switch (opKind)
		{
			case SyntaxKind.AssignmentExpression:
			case SyntaxKind.AddAssignmentExpression:
				return Precedence.Assignment;
			case SyntaxKind.MultiplyExpression:
			case SyntaxKind.DivideExpression:
				return Precedence.Multiplicative;
			case SyntaxKind.AddExpression:
			case SyntaxKind.SubtractExpression:
				return Precedence.Additive;
			case SyntaxKind.EqualsExpression:
				return Precedence.Equality;
			case SyntaxKind.GreaterThanExpression:
			case SyntaxKind.GreaterThanOrEqualExpression:
			case SyntaxKind.LessThanExpression:
			case SyntaxKind.LessThanOrEqualExpression:
				return Precedence.Relational;
			case SyntaxKind.NumericLiteralExpression:
			case SyntaxKind.FalseLiteralExpression:
			case SyntaxKind.TrueLiteralExpression:
				return Precedence.Primary;
			default:
				throw new NotImplementedException($"{opKind} have undefined precedence");
		}
	}

	public static DefaultType GetDefaultTypeByToken(Token keyword)
	{
		return keyword.Kind switch
		{
			SyntaxKind.I8Keyword => DefaultType.I8,
			SyntaxKind.I16Keyword => DefaultType.I16,
			SyntaxKind.I32Keyword => DefaultType.I32,
			SyntaxKind.I64Keyword => DefaultType.I64,
			SyntaxKind.U8Keyword => DefaultType.U8,
			SyntaxKind.U16Keyword => DefaultType.U16,
			SyntaxKind.U32Keyword => DefaultType.U32,
			SyntaxKind.U64Keyword => DefaultType.U64,
			SyntaxKind.F16Keyword => DefaultType.F16,
			SyntaxKind.F32Keyword => DefaultType.F32,
			SyntaxKind.F64Keyword => DefaultType.F64,
			SyntaxKind.VoidKeyword => DefaultType.Void,
			SyntaxKind.ExclamationToken => DefaultType.NeverReturn,
			_ => throw new InvalidEnumArgumentException(),
		};
	}
}