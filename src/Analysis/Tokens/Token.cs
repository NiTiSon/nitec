namespace NiteCompiler.Analysis.Tokens;

public abstract class Token
{
	public readonly TokenKind Kind;
	public readonly TextAnchor Location;
	// public readonly string Content; // Bad - Substring allocates new memory for a retusa (return value)!
	protected Token(TokenKind kind, TextAnchor location)
	{
		Kind = kind;
		Location = location;
	}

	public override string ToString()
		=> $"{Kind}:{GetType().Name} @{Location}";
}