using Microsoft.Extensions.Primitives;
using NiteCompiler.Analysis.Text;

namespace NiteCompiler.Analysis.Syntax.Tokens;

public sealed class OperatorToken : Token
{
	public OperatorToken(SyntaxKind kind, TextAnchor location, StringSegment content) : base(kind, location, content)
	{
	}

	public override string ToString()
		=> $"operator {Kind} @{Location}";
}