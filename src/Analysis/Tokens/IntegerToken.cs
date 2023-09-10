namespace NiteCompiler.Analysis.Tokens;

public sealed class IntegerToken : Token
{
	public readonly string Content;
	public IntegerToken(TextAnchor position, string content)
		: base(TokenKind.BooleanLiteral, position)
	{
		Content = content;
	}

	public override string ToString()
		=> $"Int: '{Content}' @{Location}";
}