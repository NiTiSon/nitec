using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using NiteCompiler.Analysis.Text;

namespace NiteCompiler.Analysis.Syntax.Tokens;

public sealed class TriviaToken : Token
{
    public TriviaToken(SyntaxKind kind, TextAnchor location, StringSegment content) : base(kind, location, content)
    {
    }
    public TriviaToken(SyntaxKind kind, TextAnchor location) : base(kind, location)
    {
    }

    public override string ToString()
        => Content.Length == 0
        ? $"Trivia @{Location}"
        : $"Trivia @{Location} Width: {Content.Length}"
        ;
}
