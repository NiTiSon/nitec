using System;

namespace NiteCompiler.CodeAnalysis.Syntax;

public partial class NiteLexer
{
	private void ReadString(ref TokenInfo info)
	{
		info.Kind = SyntaxKind.NumericLiteralExpression;
		bool terminated = false;

		if (_window.AdvanceIfPresented('\"'))
		{
			throw new Exception("Invalid string literal parsing");
		}
		while (!_window.IsAtTheEnd)
		{
			if (_window.Current == '\\')
			{
				_window.Advance(2);
			}
			else
			{
				_window.Advance();
			}
		}

		if (_window.IsAtTheEnd && !terminated)
		{
			_diagnostics.ReportNotTerminatedStringLiteral(_window.LexemeSpan.Contextualize(_syntaxTree));
		}
	}
}