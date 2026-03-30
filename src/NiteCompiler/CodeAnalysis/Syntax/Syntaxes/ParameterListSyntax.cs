using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class ParameterListSyntax : SyntaxNode
{
	public Token OpenBrace { get; }
	public SyntaxList<ParameterSyntax> Parameters { get; }
	public Token CloseBrace { get; }

	public override TextSpan Span => TextSpan.FromBounds(OpenBrace.Span, CloseBrace.Span);
	public override NodeKind Kind => NodeKind.ParameterList;

	internal ParameterListSyntax(SyntaxTree tree, Token openBrace, SyntaxList<ParameterSyntax> parameters, Token closeBrace)
		: base(tree)
	{
		OpenBrace = openBrace;
		Parameters = parameters;
		CloseBrace = closeBrace;
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
		yield return OpenBrace;
		yield return Parameters;
		yield return CloseBrace;
	}
}