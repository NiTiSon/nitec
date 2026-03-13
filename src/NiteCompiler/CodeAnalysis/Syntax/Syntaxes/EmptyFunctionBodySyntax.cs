using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class EmptyFunctionBodySyntax : FunctionBodySyntax
{
	public Token SemicolonToken { get; }

	internal override Token ClosingToken => SemicolonToken;

	public EmptyFunctionBodySyntax(SyntaxTree tree, Token semicolonToken) : base(tree)
	{
		SemicolonToken = semicolonToken;
	}

	public override TextSpan Span => SemicolonToken.Span;
	public override NodeKind Kind => NodeKind.EmptyFunctionBody;

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitEmptyFunctionBody(this);
	public override void Accept(SyntaxVisitor visitor) => visitor.VisitEmptyFunctionBody(this);

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return SemicolonToken;
	}
}