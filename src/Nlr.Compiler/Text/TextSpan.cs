namespace Nlr.Compiler.Text;

public readonly record struct TextSpan
{
	public uint Begin { get; }

	public uint Length { get; }

	public uint End => Begin + Length;

	public TextSpan(uint begin, uint length)
	{
		Begin = begin;
		Length = length;
	}

	public static TextSpan FromBounds(uint start, uint end)
	{
		return new TextSpan(start, end - start);
	}

	public override string? ToString()
	{
		return $"{{{Begin}..{End}, Length: {Length}}}";
	}
}