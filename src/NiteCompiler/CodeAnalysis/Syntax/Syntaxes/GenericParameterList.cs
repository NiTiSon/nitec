using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class GenericParameterListSyntax : SyntaxNode
{
	public Token OpenToken { get; }
	public SyntaxList<GenericParameterSyntax> Parameters { get; }
	public Token CloseToken { get; }

	public override TextSpan Span => TextSpan.FromBounds(OpenToken.Span, CloseToken.Span);
	public override NodeKind Kind => NodeKind.GenericParameterList;

	internal GenericParameterListSyntax(SyntaxTree tree,
		Token openToken, SyntaxList<GenericParameterSyntax> parameters, Token closeToken) : base(tree)
	{
		OpenToken = openToken;
		Parameters = parameters;
		CloseToken = closeToken;
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return OpenToken;
		yield return Parameters;
		yield return OpenToken;
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitGenericParameterList(this);
	public override void Accept(SyntaxVisitor visitor) => visitor.VisitGenericParameterList(this);
}