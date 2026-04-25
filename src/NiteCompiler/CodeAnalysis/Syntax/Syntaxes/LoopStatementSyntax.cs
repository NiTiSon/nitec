using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class LoopStatementSyntax : StatementSyntax
{
	public Token LoopKeyword { get; }
	public StatementSyntax Body { get; }

	public override TextSpan Span => TextSpan.FromBounds(LoopKeyword.Span, Body.Span);
	public override NodeKind Kind => NodeKind.LoopStatement;

	internal LoopStatementSyntax(SyntaxTree tree, Token loop, StatementSyntax body) : base(tree)
	{
		LoopKeyword = loop;
		Body = body;
	}

	public override void Accept(SyntaxVisitor visitor)
	{
		visitor.VisitLoopStatement(this);
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default
	{
		return visitor.VisitLoopStatement(this);
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return LoopKeyword;
		yield return Body;
	}
}