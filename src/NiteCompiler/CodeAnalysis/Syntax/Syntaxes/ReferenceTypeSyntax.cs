using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class ReferenceTypeSyntax : TypeSyntax
{
	public Token AmpersandToken { get; }
	public LifetimeSyntax? Lifetime { get; }
	public Token? ConstToken { get; }
	public Token? QuestionToken { get; }
	public TypeSyntax ElementSyntax { get; }

	public override NodeKind Kind => NodeKind.ReferenceType;
	public override TextSpan Span => TextSpan.FromBounds(AmpersandToken.Span, ElementSyntax.Span);

	internal ReferenceTypeSyntax(SyntaxTree tree, Token ampersandToken, LifetimeSyntax? lifetime, Token? constToken, Token? questionToken, TypeSyntax elementSyntax)
		: base(tree)
	{
		AmpersandToken = ampersandToken;
		ConstToken = constToken;
		QuestionToken = questionToken;
		ElementSyntax = elementSyntax;
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return AmpersandToken;
		if (ConstToken != null) yield return ConstToken;
		if (QuestionToken != null) yield return QuestionToken;
		yield return ElementSyntax;
	}

	public override void Accept(SyntaxVisitor visitor)
	{
		visitor.VisitReferenceType(this);
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default
	{
		return visitor.VisitReferenceType(this);
	}
}