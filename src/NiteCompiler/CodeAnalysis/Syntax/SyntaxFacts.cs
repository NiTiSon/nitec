using System;

namespace NiteCompiler.CodeAnalysis.Syntax;

internal static class SyntaxFacts
{
	public static ReadOnlySpan<SyntaxKind> AccessKeywords => [
		SyntaxKind.PublicKeyword,
		SyntaxKind.ProtectedKeyword,
		SyntaxKind.InternalKeyword,
		SyntaxKind.PrivateKeyword,
		SyntaxKind.FamilyKeyword,
		SyntaxKind.FriendKeyword,
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
        }
    }
}