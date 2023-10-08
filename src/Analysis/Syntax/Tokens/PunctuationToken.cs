using Microsoft.Extensions.Primitives;
using NiteCompiler.Analysis.Text;

namespace NiteCompiler.Analysis.Syntax.Tokens;

public sealed class PunctuationToken : Token
{
	public PunctuationToken(SyntaxKind kind, TextAnchor location, StringSegment content) : base(kind, location, content)
	{
	}

	public override string ToString()
		=> $"markup: {Content} @{Location}";
}
