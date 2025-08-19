using System.Net.Http.Headers;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class IdentifierToken : Token
{
    public string Identifier { get; }

    public IdentifierToken(SyntaxKind kind, TextSpan span, string identifier) : base(kind, span)
    {
        Identifier = identifier;
    }

    public override string ToString()
    {
        return $"Identifier: {Identifier}";
    }
}