using NiteCompiler.Analysis.Text;

namespace NiteCompiler.Analysis.Tokens;

public sealed class PunctuationToken : Token
{
	public readonly string Content;
	public PunctuationToken(TextAnchor position, SyntaxKind kind)
		: base(kind, position)
	{
	}

	public override string ToString()
		=> $"Punctuation: '{GetTokenDefaultValue(Kind)}' @{Location}";

	internal static string GetTokenDefaultValue(SyntaxKind tokenKind)
		=> tokenKind switch
		{
			SyntaxKind.LBrace => "{",
			SyntaxKind.RBrace => "}",
			SyntaxKind.LParenthesis => "(",
			SyntaxKind.RParenthesis => ")",
			SyntaxKind.LBracket => "[",
			SyntaxKind.RBracket => "]",
			SyntaxKind.Semicolon => ";",
			SyntaxKind.Comma => ",",
			_ => "WRONG",
		};
}