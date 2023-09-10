namespace NiteCompiler.Analysis.Tokens;

public sealed class UnexpectedToken : Token
{
	public UnexpectedToken(TextAnchor position)
		: base(TokenKind.EndOfFile, position)
	{ }

}