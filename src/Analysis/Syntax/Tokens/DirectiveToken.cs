using Microsoft.Extensions.Primitives;
using NiteCompiler.Analysis.Text;

namespace NiteCompiler.Analysis.Syntax.Tokens;

public abstract class DirectiveToken : Token
{
	protected DirectiveToken(SyntaxKind kind, TextAnchor location, StringSegment content) : base(kind, location, content)
	{ }
}