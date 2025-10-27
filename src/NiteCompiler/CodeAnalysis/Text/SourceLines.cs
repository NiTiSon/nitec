using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Text;

/// <summary>
/// Splits a <see cref="SourceText"/> into individual lines.
/// </summary>
/// <remarks>
/// Excludes the final empty line if it has no line break or content.
/// </remarks>
public sealed class SourceLines : IEnumerable<TextLine>
{
	private readonly TextLine[] _lines;
	public SourceText Source { get; }

	public int Count => _lines.Length;

	public SourceLines(SourceText source)
	{
		Source = source ?? throw new ArgumentNullException(nameof(source));

		var lines = new List<TextLine>(capacity: 32);

		int start = 0;
		for (int i = 0; i < Source.Length;)
		{
			int width = SyntaxFacts.GetLineBreakWidth(Source, i);
			if (width == 0)
			{
				i++;
				continue;
			}

			int length = i - start + width;
			lines.Add(new TextLine(lines.Count, start, length));
			i += width;
			start = i;
		}

		if (start < Source.Length)
			lines.Add(new TextLine(lines.Count, start, Source.Length - start));

		_lines = lines.ToArray();
	}

	public TextLine GetLineByIndex(int lineIndex)
	{
		if ((uint)lineIndex >= (uint)_lines.Length)
			throw new ArgumentOutOfRangeException(nameof(lineIndex));

		return _lines[lineIndex];
	}

	public TextLine? GetLineByCharacterPosition(int characterPosition)
	{
		if (characterPosition < 0 || characterPosition > Source.Length)
			return null;

		if (characterPosition == Source.Length)
			return _lines[^1];

		int left = 0;
		int right = _lines.Length - 1;

		while (left <= right)
		{
			int mid = left + ((right - left) >> 1);
			var line = _lines[mid];

			if (characterPosition >= line.Position && characterPosition < line.End)
				return line;

			if (characterPosition < line.Position)
				right = mid - 1;
			else
				left = mid + 1;
		}

		throw new InvalidDataException("Corrupted line table: no line found for given position.");
	}

	public IEnumerator<TextLine> GetEnumerator()
	{
		for (int i = 0; i < _lines.Length; i++)
		{
			yield return _lines[i];
		}
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
