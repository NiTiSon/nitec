namespace NiteCompiler.Analysis.Tokens;

public sealed class BinaryOperatorToken : Token
{
	public BinaryOperatorToken(TokenKind operatorKind, TextAnchor position)
		: base(operatorKind, position)
	{ }
}