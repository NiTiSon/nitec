using System.Collections;
using System.Collections.Generic;
using System.IO;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Text;

/// <summary>
/// Parse source into lines.
/// </summary>
/// <remarks>
/// Does not include last empty line without line break nor content.
/// </remarks>
public sealed class SourceLines : IEnumerable<TextLine>
{
	private readonly TextLine[] _lines;
	public SourceText Source { get; }

	public SourceLines(SourceText source)
	{
		Source = source;

		List<TextLine> lines = new(capacity: 64);

		int start = 0;
		for (int i = 0; i < Source.Length; i++)
		{
			int width = SyntaxFacts.GetLineBreakWidth(Source, i);
			if (width != 0)
			{
				lines.Add(new TextLine(lines.Count, start, i - start + width));
				start = i + width;
			}
		}

		if (start < Source.Length)
		{
			lines.Add(new TextLine(lines.Count, start, Source.Length - start));
		}

		_lines = lines.ToArray();
	}

	public TextLine GetLineByIndex(int lineIndex)
	{
		return _lines[lineIndex];
	}

	public TextLine? GetLineByCharacterPosition(int characterPosition)
	{
		if (characterPosition < 0 || characterPosition > Source.Length)
		{
			return null;
		}

		if (characterPosition == Source.Length)
		{
			return _lines[^1];
		}

		// Modified binary search
		int left = 0;
		int right = _lines.Length - 1;

		while (left <= right)
		{
			int mid = left + (right - left) / 2;

			if (_lines[mid].Position <= characterPosition && _lines[mid].End > characterPosition)
			{
				return _lines[mid];
			}
			else if (_lines[mid].Position < characterPosition)
			{
				left = mid + 1;
			}
			else
			{
				right = mid - 1;
			}
		}

		throw new InvalidDataException("_lines generated with wrong values!");
	}



	public IEnumerator<TextLine> GetEnumerator()
	{
		for (int i = 0; i < _lines.Length; i++) // TextLine[].GetEnumerator returns just IEnumerator :sad:
		{
			yield return _lines[i];
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _lines.GetEnumerator();
	}
}