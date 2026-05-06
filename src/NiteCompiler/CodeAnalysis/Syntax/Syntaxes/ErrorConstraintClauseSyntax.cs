using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class ErrorConstraintClauseSyntax : LifetimeOrGenericConstraintClauseSyntax
{
	public SyntaxList<SyntaxNode> ErroredNodes { get; }

	public override TextSpan Span => ErroredNodes.Span;
	public override NodeKind Kind => NodeKind.ErrorConstraintClause;

	internal ErrorConstraintClauseSyntax(SyntaxTree tree, SyntaxList<SyntaxNode> erroredNodes) : base(tree)
	{
		ErroredNodes = erroredNodes;
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return ErroredNodes;
	}
}