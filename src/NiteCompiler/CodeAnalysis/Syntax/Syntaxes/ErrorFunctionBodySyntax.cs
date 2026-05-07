using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class ErrorFunctionBodySyntax : FunctionBodySyntax
{
	public SyntaxList<Token> ErroredNodes { get; }

	internal override Token ClosingToken => ErroredNodes[^1];

	public override NodeKind Kind =>  NodeKind.ErrorFunctionBody;
	public override TextSpan Span => ErroredNodes.Span;

	internal ErrorFunctionBodySyntax(SyntaxTree tree, SyntaxList<Token> erroredNodes) : base(tree)
	{
		ErroredNodes = erroredNodes;
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return ErroredNodes;
	}
}