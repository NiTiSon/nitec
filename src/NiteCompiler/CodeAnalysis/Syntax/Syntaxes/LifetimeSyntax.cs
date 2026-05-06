using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class LifetimeSyntax : LifetimeOrGenericParameterSyntax
{
	public Token LifetimeToken { get; }
	public string Identifier { get; }

	public override NodeKind Kind => NodeKind.Lifetime;
	public override TextSpan Span => LifetimeToken.Span;

	internal LifetimeSyntax(SyntaxTree tree, Token lifetimeToken, string identifier) : base(tree)
	{
		LifetimeToken = lifetimeToken;
		Identifier = identifier;
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return LifetimeToken;
	}

	public override void Accept(SyntaxVisitor visitor)
	{
		visitor.VisitLifetime(this);
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default
	{
		return visitor.VisitLifetime(this);
	}
}