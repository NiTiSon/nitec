namespace NiteCompiler;

public readonly struct TextAnchor
{
    public readonly uint Index;
    public readonly uint Column;
    public readonly uint Row;

    public TextAnchor()
        => this = default;

    public TextAnchor(uint position, uint column, uint row)
    {
        Index = position;
        Column = column;
        Row = row;
    }

    internal string ContentToString()
        => $"{Column}:{Row}";
    public override string ToString()
        => $"{{{ContentToString()}}}";
}