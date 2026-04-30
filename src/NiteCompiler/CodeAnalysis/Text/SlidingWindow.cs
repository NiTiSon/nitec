using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace NiteCompiler.CodeAnalysis.Text;

[DebuggerStepThrough]
public sealed class SlidingWindow
{
	private const int BufferLength = 2048;
	public const char InvalidCharacter = '\xffff';

	private readonly SourceText _sourceText;
	private readonly int _textEnd;

	private int _basis;
	private int _offset;
	private char[] _window;
	private int _windowLength;

	private int _lexemeStart;

	public int Position => _basis + _offset;

	public int Offset => _offset;

	public int LexemeStart => _basis + _lexemeStart;

	public int Width => _offset - _lexemeStart;

	public TextSpan LexemeSpan => new(LexemeStart, Width);

	public string Lexeme => new(_window, _lexemeStart, Width);

	public SlidingWindow(SourceText sourceText)
	{
		_sourceText = sourceText;
		_textEnd = sourceText.Length;

		_basis = 0;
		_offset = 0;
		_lexemeStart = 0;
		_window = new char[BufferLength];
	}

	public char Current => Peek(0);
	public char Next => Peek(1);

	public bool IsAtTheEnd => Position >= _textEnd;

	public void Start()
	{
		_lexemeStart = _offset;
	}

	public void Advance()
	{
		_offset++;
	}

	public void Advance(int offset)
	{
		_offset += offset;
	}

	public string GetText()
	{
		return new string(_window, _lexemeStart, Width);
	}

	public bool AdvanceIfPresented(char required)
	{
		if (Current != required)
			return false;

		Advance();
		return true;
	}

	public bool AdvanceIfPresented(ReadOnlySpan<char> required)
	{
		int length = required.Length;
		for (int i = 0; i < length; i++)
		{
			if (Peek(i) != required[i])
			{
				return false;
			}
		}

		Advance(length);
		return true;
	}

	public bool AdvanceWhilePresented(UnicodeCategory category)
	{
		int advanced = 0;
		while (char.GetUnicodeCategory(Peek(advanced)) == category)
		{
			advanced++;
		}

		Advance(advanced);
		return true;
	}

	public void AdvancePastNewLine()
	{
		Advance(GetNewLineWidth());
	}

	public int GetNewLineWidth()
	{
		return GetNewLineWidth(Current, Next);
	}

	public static int GetNewLineWidth(char currentChar, char nextChar)
	{
		return currentChar == '\r' && nextChar == '\n' ? 2 : 1;
	}

	public char PeekThenAdvance()
	{
		char c = Peek();
		if (c != InvalidCharacter)
		{
			Advance();
		}
		return c;
	}

	public char Peek()
	{
		if (_offset >= _windowLength
			&& !MoreChars())
		{
			return InvalidCharacter;
		}

		return _window[_offset];
	}

	public char Peek(int offset)
	{
		int position = Position;
		this.Advance(offset);

		char ch;
		if (_offset >= _windowLength
			&& !MoreChars())
		{
			ch = InvalidCharacter;
		}
		else
		{
			ch = _window[_offset];
		}

		this.Reset(position);
		return ch;
	}

	private bool MoreChars()
	{
		if (_offset >= _windowLength)
		{
			if (this.Position >= _textEnd)
			{
				return false;
			}

			// if lexeme scanning is sufficiently into the char buffer,
			// then refocus the window onto the lexeme
			if (_lexemeStart > (_windowLength / 4))
			{
				Array.Copy(_window,
					_lexemeStart,
					_window,
					0,
					_windowLength - _lexemeStart);
				_windowLength -= _lexemeStart;
				_offset -= _lexemeStart;
				_basis += _lexemeStart;
				_lexemeStart = 0;
			}

			if (_windowLength >= _window.Length)
			{
				// grow char array, since we need more contiguous space
				char[] oldWindow = _window;
				char[] newWindow = new char[_window.Length * 2];
				Array.Copy(oldWindow, 0, newWindow, 0, _windowLength);
				_window = newWindow;
			}

			int amountToRead = int.Min(_textEnd - (_basis + _windowLength),
					_window.Length - _windowLength);
			_sourceText.CopyTo(_basis + _windowLength,
				_window,
				_windowLength,
				amountToRead);
			_windowLength += amountToRead;
			return amountToRead > 0;
		}

		return true;
	}

	public void Reset(int position)
	{
		// if position is within already read character range then just use what we have
		int relative = position - _basis;
		if (relative >= 0 && relative <= _windowLength)
		{
			_offset = relative;
		}
		else
		{
			// we need to reread text buffer
			int amountToRead = int.Min(_sourceText.Length, position + _window.Length) - position;
			amountToRead = Math.Max(amountToRead, 0);
			if (amountToRead > 0)
			{
				_sourceText.CopyTo(position, _window, 0, amountToRead);
			}

			_lexemeStart = 0;
			_offset = 0;
			_basis = position;
			_windowLength = amountToRead;
		}
	}
}