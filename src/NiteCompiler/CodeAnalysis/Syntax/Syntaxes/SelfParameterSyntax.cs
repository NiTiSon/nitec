using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class SelfParameterSyntax : BaseParameterSyntax
{
	public Token SelfKeyword { get; }
	public Token DotToken { get; }
	public SimpleNameSyntax FieldName { get; }

	public override TextSpan Span => TextSpan.FromBounds(SelfKeyword.Span, FieldName.Span);
	public override NodeKind Kind => NodeKind.SelfParameter;

	internal SelfParameterSyntax(SyntaxTree tree, Token selfKeyword, Token dotToken, SimpleNameSyntax fieldName) : base(tree)
	{
		SelfKeyword = selfKeyword;
		DotToken = dotToken;
		FieldName = fieldName;
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitSelfParameter(this);
	public override void Accept(SyntaxVisitor visitor) => visitor.VisitSelfParameter(this);

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return SelfKeyword;
		yield return DotToken;
		yield return FieldName;
	}
}
