using Microsoft.Extensions.Primitives;
using NiteCompiler.Analysis.Text;

namespace NiteCompiler.Analysis.Syntax.Tokens;

public class LiteralToken : Token
{
	public LiteralToken(SyntaxKind kind, TextAnchor location, StringSegment content) : base(kind, location, content)
	{
	}
}