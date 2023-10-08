using Microsoft.Extensions.Primitives;
using NiteCompiler.Analysis.Text;

namespace NiteCompiler.Analysis.Syntax.Tokens;

public sealed class IdentifierOrKeywordToken : Token
{
	public IdentifierOrKeywordToken(SyntaxKind kind, TextAnchor location, StringSegment content) : base(kind, location, content)
	{
	}

	public override string ToString()
	{
		return Kind is SyntaxKind.Identifier
			? $"id: '{Content}' @{Location}"
			: $"kw: '{Kind.ToString()[2..].ToLower()}' @{Location}"
			;
	}
}
