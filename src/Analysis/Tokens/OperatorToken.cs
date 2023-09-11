namespace NiteCompiler.Analysis.Tokens;

public sealed class OperatorToken : Token
{
	public OperatorToken(TextAnchor position, TokenKind operatorKind)
		: base(operatorKind, position)
	{ }

    public override string ToString()
		=> $"Op: '{Kind}' @{Location}";
}