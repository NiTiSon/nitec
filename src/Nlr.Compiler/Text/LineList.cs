using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Nlr.Compiler.Text;

public sealed class LineList
{
	private readonly List<Line> lines;

	public LineList()
	{
		this.lines = new(64);
	}

	public LineList(string text) : this()
	{
		int index = 0;
		uint displayLength = 0;

		uint begin = 0;
		while (index < text.Length)
		{
			switch (text[index])
			{
				case '\n':
				{
					index++;
					if (text[index] == '\r')
					{
						index++;
					}

					break;
				}
				case '\r':
				{
					index++;
					if (text[index] == '\n')
					{
						index++;
					}

					break;
				}
				case '\t':
					displayLength += 4;
					index++;
					continue;
				default:
					displayLength++;
					index++;
					continue;
			}

			Add(new Line(begin, (uint)index - begin, displayLength));
			begin = (uint)index;
		}
	}

	public void Add(Line line)
	{
		if (lines.Count > 0)
		{
			Line previous = lines[^1];

			if (!IsConnectedCorrectly(previous, line))
			{
				throw new ArgumentException(null, nameof(line));
			}
		}

		lines.Add(line);
	}

	public LinePositionSpan GetLinePositionSpan(TextSpan span)
	{
		return new(
			GetLinePosition(span.Begin),
			GetLinePosition(span.End)
		);
	}

	public LinePosition GetLinePosition(uint characterIndex)
	{
		(Line line, uint index) = GetLine(characterIndex);
		return new LinePosition(index, characterIndex - line.Begin);
	}
	
	public (Line line, uint index) GetLine(uint characterIndex)
	{
		Span<Line> lines = CollectionsMarshal.AsSpan(this.lines); // Direct access to list data

		// Modified binary search algorithm
		uint low = 0, high = (uint)lines.Length - 1;

		while (low <= high)
		{
			uint middle = low + (high - low) / 2;

			ref Line middleLine = ref lines[(int)middle];

			if (middleLine.InBounds(characterIndex)) // If index is in the bounds of line
			{
				return (middleLine, middle);
			}

			if (middleLine.Begin > characterIndex) // If in previous lines
			{
				high = middle - 1;
			}
			else // If next lines
			{
				low = middle + 1;
			}
		}

		throw new ArgumentOutOfRangeException(nameof(characterIndex));
	}

	private bool IsConnectedCorrectly(Line previous, Line next)
	{
		return previous.End == next.Begin;
	}
}