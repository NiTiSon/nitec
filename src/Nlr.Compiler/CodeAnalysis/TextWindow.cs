using Microsoft.Extensions.Primitives;
using Nlr.Compiler.Text;
using System;
using System.Buffers;
using System.Diagnostics;

namespace Nlr.Compiler.CodeAnalysis;

public sealed class TextWindow : IDisposable
{
	public const char InvalidCharacter = char.MaxValue; // 0xFFFF is not valid Utf16 codepoint

	private readonly SourceText source;
	private readonly uint sourceEnd;

	private uint basis;
	private uint currentOffset;
	private uint windowLength;

	private uint lexemeStart;

	public uint Position => basis + currentOffset;

	public uint LexemeStartPosition => basis + lexemeStart;

	public uint LexemeLength => currentOffset - lexemeStart;

	public TextWindow(SourceText source)
	{
		this.source = source;
		basis = 0;
		currentOffset = 0;
		sourceEnd = source.Length;
		lexemeStart = 0;
	}

	public void Dispose()
	{
	}

	public void Start()
	{
		lexemeStart = currentOffset;
	}

	public bool IsAtTheEnd()
	{
		return Position >= sourceEnd;
	}

	public bool IsAtTheEnd(uint offset)
	{
		return (Position + offset) >= sourceEnd;
	}

	public bool IsAtTheEnd(int offset)
	{
		return (Position + offset) >= sourceEnd;
	}

	public uint GetNewLineWidth()
	{
		return GetNewLineWidth(Peek(), Peek(1));
	}

	public static uint GetNewLineWidth(char currentChar, char nextChar)
	{
		return currentChar == '\r' && nextChar == '\n' ? 2u : 1u;
	}

	public void Advance()
	{
		currentOffset++;
	}

	public void Advance(uint offset)
	{
		this.currentOffset += offset;
	}

	public bool TryAdvance(char required)
	{
		if (Peek() != required)
			return false;

		Advance();
		return true;
	}

	public char Next()
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
		if (IsAtTheEnd())
		{
			return InvalidCharacter;
		}

		return source[currentOffset];
	}

	public char Peek(uint offset)
	{
		if (IsAtTheEnd(offset))
		{
			return InvalidCharacter;
		}

		return source[currentOffset + offset];
	}

	public char Peek(int offset)
	{
		if (IsAtTheEnd((uint)(currentOffset + offset)))
		{
			return InvalidCharacter;
		}

		return source[(uint)(currentOffset + offset)];
	}

	public StringSegment GetText()
	{
		return source.Substring(lexemeStart, LexemeLength);
	}

	public TextSpan GetSpan()
	{
		return new(lexemeStart, LexemeLength);
	}
}