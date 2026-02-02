using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class LiteralExpressionSyntax : ExpressionSyntax
{
	public Token Token { get; }
	public override NodeKind Kind { get; }
	public override TextSpan Span => Token.Span;

	public LiteralExpressionSyntax(SyntaxTree tree, Token token, NodeKind literalType) : base(tree)
	{
		Token = token;
		Kind = literalType;
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Token;
	}
}