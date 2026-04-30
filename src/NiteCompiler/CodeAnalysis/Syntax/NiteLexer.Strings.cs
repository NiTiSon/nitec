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

		StringBuilder sb = AcquireStringBuilder();
		bool isEscaped = false;

		while (!_window.IsAtTheEnd && _window.Current != '"')
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

	private void ReadStringFormat(ref TokenInfo info)
	{

	}
}