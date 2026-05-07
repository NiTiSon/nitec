using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class EmptyFunctionBodySyntax : FunctionBodySyntax
{
	public Token SemicolonToken { get; }

	public override TextSpan Span => SemicolonToken.Span;
	public override NodeKind Kind => NodeKind.EmptyFunctionBody;

	internal override Token ClosingToken => SemicolonToken;

	internal EmptyFunctionBodySyntax(SyntaxTree tree, Token semicolonToken) : base(tree)
	{
		SemicolonToken = semicolonToken;
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return SemicolonToken;
	}
}