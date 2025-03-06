using System;
using System.Collections.Generic;

namespace Nlr.Compiler.Text;

public sealed class TextWindow
{
	private const int DefaultWindowLength = 4096;

	private readonly Source _source;
	private readonly int _sourceEnd;

	private int _basis;
	private int _offset;
	private Utf8Char[] _frame;
	private int _frameLength;

	private int _lexemeStart;

	public int Position => _basis + _offset;

	public int LexemeStartPosition => _basis + _lexemeStart;

	public int LexemeLength => _offset - _lexemeStart;

	public TextWindow(Source source)
	{
		_source = source;
		_sourceEnd = source.Length;
		_frame = new Utf8Char[DefaultWindowLength]; // WeakArrayPool.Rent(DefaultWindowLength);
	}

	public void Start()
	{
		_lexemeStart = _offset;
	}

	public bool IsAtTheEnd()
	{
		return _offset >= _frameLength && Position >= _sourceEnd;
	}

	public bool IsAtTheEnd(int offset)
	{
		return (Position + offset) >= _sourceEnd;
	}

	public void Advance()
	{
		_offset++;
	}

	public void Advance(int offset)
	{
		_offset += offset;
	}

	public bool TryAdvance(Utf8Char required)
	{
		if (Peek() != required)
			return false;

		Advance();
		return true;
	}

	public bool AdvanceIfMatches(ReadOnlySpan<byte> utf8Desired)
	{
		int length = utf8Desired.Length;

		for (int i = 0; i < length; i++)
		{
			if (Peek(i) != utf8Desired[i])
			{
				return false;
			}
		}

		Advance(length);
		return true;
	}

	public Utf8Char Next()
	{
		Utf8Char c = Peek();
		if (c != Utf8Char.InvalidChar)
		{
			Advance();
		}
		return c;
	}

	public Utf8Char Peek()
	{
		if (_offset >= _frameLength && !MoreChars())
		{
			return Utf8Char.InvalidChar;
		}

		return _frame[_offset];
	}

	public Utf8Char Peek(int delta)
	{
		int position = this.Position;
		this.Advance(delta);

		Utf8Char ch;
		if (_offset >= _frameLength && !MoreChars())
		{
			ch = Utf8Char.InvalidChar;
		}
		else
		{
			// N.B. MoreChars may update the offset.
			ch = _frame[_offset];
		}

		this.Reset(position);
		return ch;
	}

	public void Reset(int position)
	{
		// if position is within already read character range then just use what we have
		int relative = position - _basis;
		if (relative >= 0 && relative <= _frameLength)
		{
			_offset = relative;
		}
		else
		{
			// we need to reread text buffer
			int amountToRead = Math.Min(_source.Length, position + _frame.Length) - position;
			amountToRead = Math.Max(amountToRead, 0);
			if (amountToRead > 0)
			{
				_source.CopyTo(position, _frame, 0, amountToRead);
			}

			_lexemeStart = 0;
			_offset = 0;
			_basis = position;
			_frameLength = amountToRead;
		}
	}

	private bool MoreChars()
	{
		if (_offset >= _frameLength)
		{
			if (this.Position >= _sourceEnd)
			{
				return false;
			}

			// if lexeme scanning is sufficiently into the char buffer, 
			// then refocus the window onto the lexeme
			if (_lexemeStart > (_frameLength / 4))
			{
				Array.Copy(_frame,
					_lexemeStart,
					_frame,
					0,
					_frameLength - _lexemeStart);
				_frameLength -= _lexemeStart;
				_offset -= _lexemeStart;
				_basis += _lexemeStart;
				_lexemeStart = 0;
			}

			if (_frameLength >= _frame.Length)
			{
#if DEBUG
				Console.WriteLine("[TextWindows.cs]: Assert when(_frameLength >= _frame.Length)");
#endif
				// grow char array, since we need more contiguous space
				Utf8Char[] oldWindow = _frame;
				Utf8Char[] newWindow = new Utf8Char[_frame.Length * 2];
				Array.Copy(oldWindow, 0, newWindow, 0, _frameLength);
				// s_windowPool.ForgetTrackedObject(oldWindow, newWindow);
				_frame = newWindow;
			}

			int amountToRead = Math.Min(_sourceEnd - (_basis + _frameLength),
				_frame.Length - _frameLength);
			_source.CopyTo(_basis + _frameLength,
				_frame,
				_frameLength,
				amountToRead);
			_frameLength += amountToRead;
			return amountToRead > 0;
		}

		return true;
	}
}