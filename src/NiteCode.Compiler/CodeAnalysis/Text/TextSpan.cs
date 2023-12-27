namespace NiteCode.Compiler.CodeAnalysis.Text;

public readonly struct TextSpan(uint position, uint length)
{
	public readonly uint Position { get; } = position;
	public readonly uint Length { get; } = length;
	public readonly uint EndPosition => Position + Length;

	public static TextSpan FromBounds(uint start, uint end)
	{
		uint length = end - start;
		return new TextSpan(start, length);
	}

	public bool OverlapsWith(TextSpan span)
	{
		return Position < span.EndPosition
			&& EndPosition > span.Position;
	}

	public override string ToString()
		=> $"{Position}..{EndPosition}";
}