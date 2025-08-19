using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public class Token
{
    public SyntaxKind Kind { get; }
    public TextSpan Span { get; }

    public Token(SyntaxKind kind, TextSpan span)
    {
        Kind = kind;
        Span = span;
    }

    public override string ToString()
    {
        return $"{Kind} @{Span}";
    }
}