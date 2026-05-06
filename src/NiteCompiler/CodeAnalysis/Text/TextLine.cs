using System.Diagnostics;
using System.Runtime.Serialization;

namespace NiteCompiler.CodeAnalysis.Text;

/// <summary>
/// Struct describes a line within text.
/// </summary>
public readonly struct TextLine
{
	/// <summary>
	/// Zero-based index of line.
	/// </summary>
	public readonly int Index;

	/// <summary>
	/// Position of the first character within this line.
	/// </summary>
	public readonly int Position;

	/// <summary>
	/// Name is very descriptive.
	/// </summary>
	public readonly int LengthIncludingLineBreak;

	public int End => Position + LengthIncludingLineBreak;

	public TextSpan LineSpan => new(Position, LengthIncludingLineBreak);
	public bool IsEmpty => LengthIncludingLineBreak == 0;

	public int HumanReadableLineNumber => Index + 1;

	public TextLine(int index, int position, int lengthIncludingLineBreak)
	{
		Index = index;
		Position = position;
		LengthIncludingLineBreak = lengthIncludingLineBreak;
	}

	public int GetColumnIndex(int globalPosition)
	{
		return globalPosition - Position;
	}

	public override string ToString()
	{
		return $"#{HumanReadableLineNumber}: {LineSpan} Width:{LineSpan.Length}";
	}
}