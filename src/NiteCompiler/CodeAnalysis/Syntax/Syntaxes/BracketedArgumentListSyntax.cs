using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class BracketedArgumentListSyntax : SyntaxNode
{
	public Token OpenBracket { get; }
	public SyntaxList<ExpressionSyntax> Arguments { get; }
	public Token CloseBracket { get; }

	public override NodeKind Kind => NodeKind.ArgumentList;
	public override TextSpan Span => TextSpan.FromBounds(OpenBracket.Span, CloseBracket.Span);

	internal BracketedArgumentListSyntax(SyntaxTree tree, Token openBracket, SyntaxList<ExpressionSyntax> arguments, Token closeBracket)
		: base(tree)
	{
		OpenBracket = openBracket;
		Arguments = arguments;
		CloseBracket = closeBracket;
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return OpenBracket;
		yield return Arguments;
		yield return CloseBracket;
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default =>
		visitor.VisitBracketedArgumentList(this);

	public override void Accept(SyntaxVisitor visitor) => visitor.VisitBracketedArgumentList(this);
}