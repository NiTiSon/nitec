using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class LifetimeParameterSyntax : GenericOrLifetimeParameterSyntax
{
	public LifetimeSyntax Lifetime { get; }

	public override NodeKind Kind => NodeKind.GenericLifetimeParameter;
	public override TextSpan Span => Lifetime.Span;

	internal LifetimeParameterSyntax(SyntaxTree tree, LifetimeSyntax lifetime) : base(tree)
	{
		Lifetime = lifetime;
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Lifetime;
	}

	public override void Accept(SyntaxVisitor visitor)
	{
		visitor.VisitLifetimeParameter(this);
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default
	{
		return visitor.VisitLifetimeParameter(this);
	}
}