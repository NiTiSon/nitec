using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class ArgumentListSyntax : SyntaxNode
{
	public Token OpenParen { get; }
	public SyntaxList<ExpressionSyntax> Arguments { get; }
	public Token CloseParen { get; }

	public override NodeKind Kind => NodeKind.ArgumentList;
	public override TextSpan Span => TextSpan.FromBounds(OpenParen.Span, CloseParen.Span);

	internal ArgumentListSyntax(SyntaxTree tree, Token openParen, SyntaxList<ExpressionSyntax> arguments, Token closeParen)
		: base(tree)
	{
		OpenParen = openParen;
		Arguments = arguments;
		CloseParen = closeParen;
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return OpenParen;
		yield return Arguments;
		yield return CloseParen;
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default =>
		visitor.VisitArgumentList(this);

	public override void Accept(SyntaxVisitor visitor) => visitor.VisitArgumentList(this);
}