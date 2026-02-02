using System.Globalization;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public partial class NiteLexer
{
	private static bool IsBeginIdentifier(char c)
	{
		if (char.IsAsciiDigit(c) || c is '_')
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
		if (char.IsAsciiLetterOrDigit(c) || c is '_')
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
		info.Kind = TokenKind.IdentifierOrKeyword;
		while (IsContinueIdentifier(_window.Current))
		{
			_window.Advance();
		}
	}
	private void ReadIdentifier(ref TokenInfo info)
	{
		if (IsBeginIdentifier(_window.Current))
		{
			info.Kind = TokenKind.IdentifierOrKeyword;
			_window.Advance();
			while (IsContinueIdentifier(_window.Current))
			{
				_window.Advance();
			}
		}
	}

	private void ReadLifetimeIdentifierOrCharacter(ref TokenInfo info)
	{
		if (_window.Current == '\'')
		{
			_window.Advance();
			ReadIdentifierSkipFirst(ref info);
			if (_window.Current == '\'')
			{
				_window.Advance();
				info.Kind = TokenKind.CharacterLiteral;
			}
			else
			{
				info.Kind = TokenKind.LifetimeIdentifier;
			}
		}
	}
}