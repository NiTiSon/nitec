namespace NiteCompiler.Analysis.Tokens;

public sealed class IdentifierToken : Token
{
	public readonly string Content;
	public IdentifierToken(TextAnchor position, string content)
		: base(TokenKind.BooleanLiteral, position)
	{
		Content = content;
	}

	public override string ToString()
		=> $"Id: '{Content}' @{Location}";
}