using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class ParameterListSyntax : SyntaxNode
{
	public Token OpenParen { get; }
	public SyntaxList<ParameterSyntax> Parameters { get; }
	public Token CloseParen { get; }

	public override TextSpan Span => TextSpan.FromBounds(OpenParen.Span, CloseParen.Span);
	public override NodeKind Kind => NodeKind.ParameterList;

	internal ParameterListSyntax(SyntaxTree tree, Token openParen, SyntaxList<ParameterSyntax> parameters, Token closeParen)
		: base(tree)
	{
		OpenParen = openParen;
		Parameters = parameters;
		CloseParen = closeParen;
	}

	public override void Accept(SyntaxVisitor visitor)
	{
		visitor.VisitParameterList(this);
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default
	{
		return visitor.VisitParameterList(this);
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return OpenParen;
		yield return Parameters;
		yield return CloseParen;
	}
}