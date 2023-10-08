using Microsoft.Extensions.Primitives;
using NiteCompiler.Analysis.Text;

namespace NiteCompiler.Analysis.Syntax.Tokens;

public abstract class Token : SyntaxNode
{
	public readonly TextAnchor Location;
	public readonly StringSegment Content;

	protected Token(SyntaxKind kind, TextAnchor location, StringSegment content) : base(kind)
	{
		Location = location;
		Content = content;
	}

	protected Token(SyntaxKind kind, TextAnchor location, string content)
		: this(kind, location, new StringSegment(content)) { }

	protected Token(SyntaxKind kind, TextAnchor location)
		: this(kind, location, StringSegment.Empty) { }

	public override string ToString()
		=> $"{(Content.Length is 0 ? Kind : Content)} @{Location}";
}