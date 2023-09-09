namespace NiteCompiler.Analysis;

public abstract class Token
{
    public abstract TokenKind Kind { get; }

    public override string ToString()
    {
        return Kind.ToString();
    }
}

public sealed class TestToken : Token
{
    public override TokenKind Kind
        => TokenKind.BooleanLiteral;
}