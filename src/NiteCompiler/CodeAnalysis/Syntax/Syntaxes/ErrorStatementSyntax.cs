using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class ErrorStatementSyntax : StatementSyntax
{
	public SyntaxList<Token> ErrorNodes { get; }

	public override TextSpan Span => ErrorNodes.Span;
	public override NodeKind Kind => NodeKind.ErrorStatement;

	internal ErrorStatementSyntax(SyntaxTree tree, SyntaxList<Token> errorNodes) : base(tree)
	{
		ErrorNodes = errorNodes;
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default
	{
		return visitor.VisitErrorStatement(this);
	}

	public override void Accept(SyntaxVisitor visitor)
	{
		visitor.VisitErrorStatement(this);
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return ErrorNodes;
	}
}
