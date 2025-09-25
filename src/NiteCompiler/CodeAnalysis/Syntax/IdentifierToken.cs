using System.Net.Http.Headers;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class IdentifierToken : Token
{
    public new SyntaxKind ContextualKind { get; }
    public string Identifier { get; }

    public IdentifierToken(SyntaxKind kind, SyntaxKind contextualKind, TextSpan span, string identifier) : base(kind, span)
    {
        ContextualKind = contextualKind;
        Identifier = identifier;
    }

    public override string ToString()
    {
        return $"Identifier: {Identifier} @{Span}";
    }
}