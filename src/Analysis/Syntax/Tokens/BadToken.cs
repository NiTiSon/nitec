using NiteCompiler.Analysis.Text;

namespace NiteCompiler.Analysis.Syntax.Tokens;

public sealed class BadToken : Token
{
	public BadToken(TextAnchor position)
		: base(SyntaxKind.WrongToken, position)
	{ }
}