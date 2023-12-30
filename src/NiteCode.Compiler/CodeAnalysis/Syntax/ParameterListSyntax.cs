using System.Collections.Generic;
using System.Collections.Immutable;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public sealed class ParameterListSyntax(SyntaxTree tree, SyntaxToken open, ImmutableArray<ParameterSyntax> parameters, SyntaxToken closed) : SyntaxNode(tree)
{
	public SyntaxToken OpenParenthesis { get; } = open;
	public ImmutableArray<ParameterSyntax> Parameters { get; } = parameters;
	public SyntaxToken ClosedParenthesis { get; } = closed;

	public override SyntaxKind Kind => SyntaxKind.ParameterList;

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return OpenParenthesis;
		
		foreach(var parameter in Parameters)
			yield return parameter;

		yield return ClosedParenthesis;
	}
}