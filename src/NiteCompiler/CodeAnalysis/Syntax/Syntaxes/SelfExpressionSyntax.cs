using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class SelfExpressionSyntax : ExpressionSyntax
{
	public Token SelfKeyword { get; }

	public override TextSpan Span => SelfKeyword.Span;
	public override NodeKind Kind => NodeKind.SelfExpression;

	internal SelfExpressionSyntax(SyntaxTree tree, Token selfKeyword) : base(tree)
	{
		SelfKeyword = selfKeyword;
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitSelfExpression(this);
	public override void Accept(SyntaxVisitor visitor) => visitor.VisitSelfExpression(this);

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return SelfKeyword;
	}
}
