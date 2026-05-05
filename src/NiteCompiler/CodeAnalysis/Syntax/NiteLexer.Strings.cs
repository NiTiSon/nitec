using System.Diagnostics;
using System.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

internal partial class NiteLexer
{
	private StringBuilder AcquireStringBuilder()
	{
		StringBuilder sb = _cache.StringBuilder;
		sb.Clear();
		return sb;
	}

	private void ReadString(ref TokenInfo info)
	{
		Debug.Assert(_window.Current == '\"');
		_window.Advance();

		info.Kind = TokenKind.StringLiteral;
		info.StringType = StringLiteralType.None;

		StringBuilder sb = AcquireStringBuilder();
		bool ignoredEscapeFlag = false;

		while (!_window.IsAtTheEnd && _window.Current != '"')
		{
			if (_window.Current is '\r' or '\n')
			{
				_diagnostics.ReportUnterminatedStringLiteral(_window.LexemeSpan.Contextualize(_syntaxTree));
				return;
			}

			sb.Append(ReadCharacterSymbol(ref ignoredEscapeFlag));
		}

		if (_window.Current == '"')
		{
			_window.Advance();
			ReadStringFormat(ref info);
		}
		else
		{
			_diagnostics.ReportUnterminatedStringLiteral(_window.LexemeSpan.Contextualize(_syntaxTree));
		}
	}

	private void ReadStringFormat(ref TokenInfo info)
	{
		if (_window.Current != 'u')
		{
			return;
		}

		char next = _window.Next;
		char third = _window.Peek(2);

		if (next == '8')
		{
			info.StringType = StringLiteralType.Unicode8;
			_window.Advance(2);
		}
		else if (next == '1' && third == '6')
		{
			info.StringType = StringLiteralType.Unicode16;
			_window.Advance(3);
		}
		else if (next == '3' && third == '2')
		{
			info.StringType = StringLiteralType.Unicode32;
			_window.Advance(3);
		}
	}

	private void ReadCharacterOrLifetime(ref TokenInfo info)
	{
		Debug.Assert(_window.Current == '\'');

		ResetPoint rp = GetResetPoint();
		_window.Advance();
		bool isDefinitelyACharacter = false; // escape sequences are allowed only in strings or character literals
		ReadCharacterSymbol(ref isDefinitelyACharacter);

		if (isDefinitelyACharacter)
		{
			Reset(rp);
			ReadCharacter(ref info);
		}
		else if (_window.Current == '\'')
		{
			Reset(rp);
			ReadCharacter(ref info);
		}
		else // otherwise definitely a lifetime identifier
		{
			Reset(rp);
			ReadLifetime(ref info);
		}
	}

	private void ReadLifetime(ref TokenInfo info)
	{
		Debug.Assert(_window.Current == '\'');
		info.Kind = TokenKind.LifetimeIdentifier;

		_window.Advance();
		StringBuilder sb = AcquireStringBuilder();
		char c = _window.Current;
		if (IsBeginIdentifier(c, allowDigits: false))
		{
			int width = 1;
			sb.Append(c);
			while (IsContinueIdentifier(c = _window.Peek(width)))
			{
				sb.Append(c);
				width++;
			}

			_window.Advance(width);
		}
		else
		{
			_diagnostics.ReportEmptyLifetimeName(_window.LexemeSpan.Contextualize(_syntaxTree));
		}
	}

	private void ReadCharacter(ref TokenInfo info)
	{
		Debug.Assert(_window.Current == '\'');
		_window.Advance();

		info.Kind = TokenKind.CharacterLiteral;
		info.StringType = StringLiteralType.None;
		StringBuilder sb = AcquireStringBuilder();

		bool ignoredEscapeFlag = false;
		while (!_window.IsAtTheEnd && _window.Current != '\'')
		{
			if (_window.Current is '\r' or '\n')
			{
				_diagnostics.ReportUnterminatedStringLiteral(_window.LexemeSpan.Contextualize(_syntaxTree));
				return;
			}

			sb.Append(ReadCharacterSymbol(ref ignoredEscapeFlag));
		}

		if (_window.Current == '\'')
		{
			_window.Advance();
			ReadStringFormat(ref info);
		}
		else
		{
			_diagnostics.ReportUnterminatedStringLiteral(_window.LexemeSpan.Contextualize(_syntaxTree));
		}
	}
}