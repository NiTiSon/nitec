using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class MemberAccessExpressionSyntax : ExpressionSyntax
{
	public ExpressionSyntax Expression { get; }
	public Token DotToken { get; }
	public SimpleNameSyntax Name { get; }

	public override TextSpan Span => TextSpan.FromBounds(Expression.Span, Name.Span);
	public override NodeKind Kind => NodeKind.MemberAccessExpression;

	internal MemberAccessExpressionSyntax(SyntaxTree tree, ExpressionSyntax expression, Token dotToken, SimpleNameSyntax name) : base(tree)
	{
		Expression = expression;
		DotToken = dotToken;
		Name = name;
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitMemberAccessExpression(this);
	public override void Accept(SyntaxVisitor visitor) => visitor.VisitMemberAccessExpression(this);

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Expression;
		yield return DotToken;
		yield return Name;
	}
}
