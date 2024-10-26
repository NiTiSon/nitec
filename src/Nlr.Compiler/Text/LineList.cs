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
			else if (middleLine.Begin > characterIndex) // If in previous lines
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