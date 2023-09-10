namespace NiteCompiler;

public readonly struct TextAnchor
{
	public readonly uint Index;
	public readonly uint Line;
	public readonly uint Column;

	public TextAnchor()
		=> this = default;

	public TextAnchor(uint position, uint line, uint column)
	{
		Index = position;
		Line = line;
		Column = column;
	}

	internal string ContentToString()
		=> $"{Line}:{Column}";
	public override string ToString()
		=> $"{{{ContentToString()}}}";
}