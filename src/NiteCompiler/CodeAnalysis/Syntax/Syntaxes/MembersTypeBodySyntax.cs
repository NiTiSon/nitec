using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class MembersTypeBodySyntax : TypeBodySyntax
{
	public Token OpenBrace { get; }
	public SyntaxList<MemberSyntax> Members { get; }
	public Token CloseBrace { get; }

	internal override Token ClosingToken => CloseBrace;

	public MembersTypeBodySyntax(SyntaxTree tree, Token openBrace, SyntaxList<MemberSyntax> members, Token closeBrace) : base(tree)
	{
		OpenBrace = openBrace;
		Members = members;
		CloseBrace = closeBrace;
	}

	public override TextSpan Span => TextSpan.FromBounds(OpenBrace.Span, CloseBrace.Span);
	public override NodeKind Kind => NodeKind.TypeBody;

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitMembersTypeBody(this);
	public override void Accept(SyntaxVisitor visitor) => visitor.VisitMembersTypeBody(this);

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return OpenBrace;
		foreach (ItemSyntax item in Members)
		{
			yield return item;
		}
		yield return CloseBrace;
	}
}