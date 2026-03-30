using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class ElseClauseSyntax : SyntaxNode
{
	public Token ElseToken { get; }
	public StatementSyntax ElseStatement { get; }
	public override TextSpan Span => TextSpan.FromBounds(ElseToken.Span, ElseToken.Span);
	public override NodeKind Kind => NodeKind.ElseClause;

	public ElseClauseSyntax(SyntaxTree tree, Token @else, StatementSyntax elseStatement) : base(tree)
	{
		ElseToken = @else;
		ElseStatement = elseStatement;
	}

	public override void Accept(SyntaxVisitor visitor)
	{
		visitor.VisitElseClause(this);
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default
	{
		return visitor.VisitElseClause(this);
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return ElseToken;
		yield return ElseStatement;
	}
}