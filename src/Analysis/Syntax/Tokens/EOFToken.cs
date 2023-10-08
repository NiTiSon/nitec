using NiteCompiler.Analysis.Text;

namespace NiteCompiler.Analysis.Syntax.Tokens;

public sealed class EOFToken : Token
{
	public EOFToken(TextAnchor position)
		: base(SyntaxKind.EndOfFile, position)
	{ }
}