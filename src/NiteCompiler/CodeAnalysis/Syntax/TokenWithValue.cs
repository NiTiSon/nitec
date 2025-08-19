using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class TokenWithValue<T> : Token
{
    public T Value { get; }

    public TokenWithValue(SyntaxKind kind, TextSpan span, T value) : base(kind, span)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}