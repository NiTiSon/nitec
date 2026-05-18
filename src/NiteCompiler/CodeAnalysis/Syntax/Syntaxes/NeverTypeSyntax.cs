using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class NeverTypeSyntax : TypeSyntax
{
	public Token NeverToken { get; }

	public override NodeKind Kind => NodeKind.NeverType;
	public override TextSpan Span => NeverToken.Span;

	internal NeverTypeSyntax(SyntaxTree tree, Token neverToken) : base(tree)
	{
		NeverToken = neverToken;
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitNeverType(this);
	public override void Accept(SyntaxVisitor visitor) => visitor.VisitNeverType(this);

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return NeverToken;
	}
}