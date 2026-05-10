using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class ErrorTypeBodySyntax : TypeBodySyntax
{
	public SyntaxList<SyntaxNode> ErrorNodes { get; }

	public override TextSpan Span => ErrorNodes.Span;
	public override NodeKind Kind => NodeKind.ErrorTypeBody;

	internal ErrorTypeBodySyntax(SyntaxTree tree, SyntaxList<SyntaxNode> errorNodes) : base(tree)
	{
		ErrorNodes = errorNodes;
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return ErrorNodes;
	}
}