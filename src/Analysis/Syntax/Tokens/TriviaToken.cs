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
		? $"trivia @{Location}"
		: $"trivia @{Location} Width: {Content.Length}"
		;
}
