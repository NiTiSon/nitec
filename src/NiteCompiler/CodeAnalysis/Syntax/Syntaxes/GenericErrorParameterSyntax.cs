using System.Collections.Generic;
using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class GenericErrorParameterSyntax : GenericParameterSyntax
{
	public SyntaxList<SyntaxNode> ErrorNodes { get; }

	public override NodeKind Kind => NodeKind.GenericErrorParameter;
	public override TextSpan Span =>  TextSpan.FromBounds(ErrorNodes.Span, ErrorNodes.Span);

	internal GenericErrorParameterSyntax(SyntaxTree tree, SyntaxList<SyntaxNode> errorNodes) : base(tree)
	{
		ErrorNodes = errorNodes;
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return ErrorNodes;
	}
}