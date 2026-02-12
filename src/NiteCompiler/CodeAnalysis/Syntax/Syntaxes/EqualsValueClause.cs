using System.Collections.Generic;
using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class EqualsValueClause : SyntaxNode
{
	public Token EqualsToken { get; }
	public ExpressionSyntax Expression { get; }

	public override TextSpan Span => TextSpan.FromBounds(EqualsToken.Span, Expression.Span);
	public override NodeKind Kind => NodeKind.EqualsValueClause;

	internal EqualsValueClause(SyntaxTree tree, Token equalsToken, ExpressionSyntax expression) : base(tree)
	{
		Debug.Assert(equalsToken.TKind == TokenKind.Equal);
		EqualsToken = equalsToken;
		Expression = expression;
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return EqualsToken;
		yield return Expression;
	}
}