namespace NiteCompiler.Analysis.Tokens;

public sealed class EOFToken : Token
{
	public EOFToken(TextAnchor position)
		: base(TokenKind.EndOfFile, position)
	{ }


}