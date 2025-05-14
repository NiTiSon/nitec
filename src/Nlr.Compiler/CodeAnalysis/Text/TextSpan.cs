namespace Nlr.Compiler.CodeAnalysis.Text;

public readonly struct TextSpan
{
	public readonly int Start;
	public readonly int Length;
	public readonly int End => Start + Length;

	public TextSpan(int start, int length)
	{
		Start = start;
		Length = length;
	}
	
	public override string ToString() => $"{Start}..{End}";
}