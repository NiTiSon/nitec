using System;

namespace NiteCompiler.CodeAnalysis.Syntax;

public partial class NiteLexer
{
	private void ReadNumber(ref TokenInfo info)
	{
		info.Kind = TokenKind.NumberLiteral;
		info.LiteralType = NumericLiteralType.None;
		info.LiteralFormat = NumericLiteralFormat.Integer;

		if (_window.Current == '0')
		{
			switch (_window.Next)
			{
				case 'x':
					_window.Advance(2);
					ReadHexInteger(ref info);
					ReadNumericTypeSuffix(ref info);
					return;
				case 'b':
					_window.Advance(2);
					ReadBinaryInteger(ref info);
					ReadNumericTypeSuffix(ref info);
					return;
			}
		}

		ReadDecimalNumber(ref info);
		ReadNumericTypeSuffix(ref info);
	}

	private void ReadDecimalNumber(ref TokenInfo info)
	{
		while (SyntaxFacts.IsValidDecimalDigit(_window.Current))
			_window.Advance();

		if (_window.Current == '.')
		{
			// Look ahead: '.' followed by digit means fractional
			if (SyntaxFacts.IsValidDecimalDigit(_window.Next))
			{
				info.LiteralFormat = NumericLiteralFormat.Float;
				_window.Advance(); // consume '.'

				while (SyntaxFacts.IsValidDecimalDigit(_window.Current))
					_window.Advance();
			}
			else
			{
				// 12. is valid float
				info.LiteralFormat = NumericLiteralFormat.Float;
				_window.Advance(); // consume '.'
			}
		}

		if (_window.Current is 'e' or 'E')
		{
			info.LiteralFormat = NumericLiteralFormat.ENotation;
			_window.Advance();

			if (_window.Current is '+' or '-')
				_window.Advance();

			while (SyntaxFacts.IsValidDecimalDigit(_window.Current))
				_window.Advance();
		}
	}

	private void ReadHexInteger(ref TokenInfo info)
	{
		while (SyntaxFacts.IsValidHexDigit(_window.Current))
			_window.Advance();
	}

	private void ReadBinaryInteger(ref TokenInfo info)
	{
		while (SyntaxFacts.IsValidBinaryDigit(_window.Current))
			_window.Advance();
	}

	private void ReadNumericTypeSuffix(ref TokenInfo info)
	{
		if (!char.IsAsciiLetter(_window.Current)) // I believe that faster than call three comparison
			return;

		Span<char> buf = stackalloc char[3];
		int len = 0;

		int max = 3;
		while (len < max && char.IsAsciiLetterOrDigit(_window.Current))
		{
			buf[len++] = _window.Current;
			_window.Advance();
		}

		switch (len)
		{
			case 3:
				switch (buf[..3])
				{
					case "u16": info.LiteralType = NumericLiteralType.U16; return;
					case "u32": info.LiteralType = NumericLiteralType.U32; return;
					case "u64": info.LiteralType = NumericLiteralType.U64; return;

					case "i16": info.LiteralType = NumericLiteralType.I16; return;
					case "i32": info.LiteralType = NumericLiteralType.I32; return;
					case "i64": info.LiteralType = NumericLiteralType.I64; return;

					case "f16": info.LiteralType = NumericLiteralType.F16; return;
					case "f32": info.LiteralType = NumericLiteralType.F32; return;
					case "f64": info.LiteralType = NumericLiteralType.F64; return;
				}

				break;
			case 2:
				switch (buf[..2])
				{
					case "u8": info.LiteralType = NumericLiteralType.U8; return;
					case "i8": info.LiteralType = NumericLiteralType.I8; return;
				}

				break;
			case 1:
				switch (buf[0])
				{
					case 'u': info.LiteralType = NumericLiteralType.Unsigned; return;
					case 'i': info.LiteralType = NumericLiteralType.Signed;   return;
				}

				break;
		}

		// If we reach here — unrecognized suffix.
		// Ideally issue diagnostic, but lexer cannot do that yet.
		// We simply ignore it (keeping literal type = None).
	}
}
