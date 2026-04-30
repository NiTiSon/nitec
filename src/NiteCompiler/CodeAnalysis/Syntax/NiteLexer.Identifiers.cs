using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

internal partial class NiteLexer
{
	private static bool IsBeginIdentifier(char c, bool allowDigits)
	{
		if ((allowDigits ? char.IsLetterOrDigit(c) : char.IsLetter(c)) ||
		    c is '_')
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

	private void ReadIdentifier(ref TokenInfo info)
	{
		if (IsBeginIdentifier(_window.Current, allowDigits: false))
		{
			info.Kind = TokenKind.IdentifierOrKeyword;
			_window.Advance();
			while (IsContinueIdentifier(_window.Current))
			{
				_window.Advance();
			}
		}
	}

	private void ReadEscapedIdentifier(ref TokenInfo info)
	{
		Debug.Assert(_window.Current == '`');
		_window.Advance();

		info.Kind = TokenKind.EscapedIdentifier;

		StringBuilder sb = AcquireStringBuilder();
		bool isEscaped = false;

		while (!_window.IsAtTheEnd && _window.Current != '`')
		{
			sb.Append(ReadCharacterSymbol(ref isEscaped));
		}

		if (_window.Current == '`')
		{
			_window.Advance();
		}
		else
		{
			_diagnostics.ReportUnterminatedEscapedIdentifier(_window.LexemeSpan.Contextualize(_syntaxTree));
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private char ReadCharacterSymbol(ref bool isEscaped)
	{
		char c = _window.Current;

		if (c == '\\')
		{
			_window.Advance();

			if (_window.IsAtTheEnd)
				return '\0';

			char e = _window.Current;
			_window.Advance();

			// TODO: Unicode point escape sequence
			if (SyntaxFacts.TryGetEscapedCharacter(ref e))
			{
				isEscaped = true;
				return e;
			}

			// TODO: report warning
			return e;
		}

		_window.Advance();
		return c;
	}
}