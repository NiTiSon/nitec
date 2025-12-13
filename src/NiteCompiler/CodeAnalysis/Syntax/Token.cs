using System;
using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Syntax;

public class Token : SyntaxNode
{
    public override SyntaxKind Kind { get; }

    public SyntaxKind ContextualKind => this is IdentifierToken identifier ? identifier.ContextualKind : SyntaxKind.None;
    public override TextSpan Span { get; }

    public Token(SyntaxKind kind, TextSpan span)
    {
        Kind = kind;
        Span = span;
    }

    public bool IsConnectedAfter(Token token)
    {
	    return Span.End == token.Span.Start;
    }

    public bool IsConnectedBefore(Token token)
    {
	    return token.IsConnectedAfter(this);
    }

    public Location CreateLocation(SyntaxTree tree)
    {
	    return Location.Create(tree, Span);
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return [];
    }
}