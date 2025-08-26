using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public class Token : SyntaxNode
{
    public override SyntaxKind Kind { get; }
    public override TextSpan Span { get; }

    public Token(SyntaxKind kind, TextSpan span)
    {
        Kind = kind;
        Span = span;
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return [];
    }
}