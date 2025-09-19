using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Syntax.Expressions;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class LiteralExpressionSyntax : ExpressionSyntax
{
	public Token Token { get; }
	public override SyntaxKind Kind { get; }

	public LiteralExpressionSyntax(Token token, SyntaxKind kind)
	{
		Token = token;
		Kind = kind;
	}

	public override TextSpan Span => Token.Span;
	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Token;
	}
}