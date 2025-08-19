using System.Globalization;

namespace NiteCompiler.CodeAnalysis.Syntax;

public partial class NiteLexer
{
    private static bool IsBeginIdentifier(char c)
    {
        if (c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or '_')
        {
            return true;
        }

        UnicodeCategory category = char.GetUnicodeCategory(c);
        
        return category is UnicodeCategory.UppercaseLetter
            or UnicodeCategory.LowercaseLetter
            or UnicodeCategory.TitlecaseLetter
            or UnicodeCategory.ModifierLetter
            or UnicodeCategory.OtherLetter
            or UnicodeCategory.LetterNumber;
    }

    private static bool IsContinueIdentifier(char c)
    {
        if (c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or '_' or >= '0' and <= '9')
        {
            return true;
        }

        UnicodeCategory category = char.GetUnicodeCategory(c);

        return category is UnicodeCategory.UppercaseLetter
            or UnicodeCategory.LowercaseLetter
            or UnicodeCategory.TitlecaseLetter
            or UnicodeCategory.ModifierLetter
            or UnicodeCategory.OtherLetter
            or UnicodeCategory.LetterNumber
            or UnicodeCategory.NonSpacingMark
            or UnicodeCategory.SpacingCombiningMark
            or UnicodeCategory.ConnectorPunctuation;
    }

    private void ReadIdentifierSkipFirst(ref TokenInfo info)
    {
        info.Kind = SyntaxKind.IdentifierToken;
        while (IsContinueIdentifier(_window.Current))
        {
            _window.Advance();
        }
    }

    private void ReadIdentifier(ref TokenInfo info)
    {
        if (IsBeginIdentifier(_window.Current))
        {
            info.Kind = SyntaxKind.IdentifierToken;
            _window.Advance();

            while (IsContinueIdentifier(_window.Current))
            {
                _window.Advance();
            }
        }
    }
}