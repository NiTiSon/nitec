using System.Diagnostics;
using System.Runtime.Serialization;

namespace NiteCompiler.CodeAnalysis.Text;

[DebuggerStepThrough]
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

	public TextLine(int index, int position, int lengthIncludingLineBreak)
	{
		Index = index;
		Position = position;
		LengthIncludingLineBreak = lengthIncludingLineBreak;
	}

	public override string ToString()
	{
		return $"#{Index+1}: {LineSpan} Width:{LineSpan.Length}";
	}
}