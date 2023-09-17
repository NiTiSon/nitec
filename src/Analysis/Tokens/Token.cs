using NiteCompiler.Analysis.Text;

namespace NiteCompiler.Analysis.Tokens;

public abstract class Token
{
	public readonly SyntaxKind Kind;
	public readonly TextAnchor Location;
	public readonly string? Content;

	protected Token(SyntaxKind kind, TextAnchor location, string content)
	{
		Kind = kind;
		Location = location;
		Content = content;
	}
	protected Token(SyntaxKind kind, TextAnchor location)
	{
		Kind = kind;
		Location = location;
	}

	public override string ToString()
		=> $"{(Content is null ? Kind : Content)} @{Location}";
}