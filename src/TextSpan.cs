namespace NiteCompiler;

public readonly struct TextSpan
{
    public readonly TextAnchor Begin;
    public readonly TextAnchor End;

    public TextSpan()
        => this = default;

    public TextSpan(TextAnchor begin, TextAnchor end)
    {
        Begin = begin;
        End = end;
    }

    public override string ToString()
        => $"{{{Begin.ContentToString()}..{End.ContentToString()}}}";
}