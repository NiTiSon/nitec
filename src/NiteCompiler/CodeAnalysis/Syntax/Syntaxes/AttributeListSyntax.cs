using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class AttributeListSyntax : SyntaxNode
{
	public Token HashToken { get; }
	public Token OpenBracketToken { get; }
	public AttributeSyntax Attribute { get; }
	public Token CloseBracketToken { get; }

	public override TextSpan Span => TextSpan.FromBounds(HashToken.Span.Start, CloseBracketToken.Span.End);
	public override NodeKind Kind => NodeKind.AttributeList;

	internal AttributeListSyntax(SyntaxTree tree, Token hashToken, Token openBracketToken,
		AttributeSyntax attribute, Token closeBracketToken) : base(tree)
	{
		HashToken = hashToken;
		OpenBracketToken = openBracketToken;
		Attribute = attribute;
		CloseBracketToken = closeBracketToken;
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default =>
		visitor.VisitAttributeList(this);

	public override void Accept(SyntaxVisitor visitor) =>
		visitor.VisitAttributeList(this);

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return HashToken;
		yield return OpenBracketToken;
		yield return Attribute;
		yield return CloseBracketToken;
	}
}
