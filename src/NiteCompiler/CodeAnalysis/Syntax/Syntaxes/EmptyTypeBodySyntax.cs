using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class EmptyTypeBodySyntax : TypeBodySyntax
{
	public Token SemicolonToken { get; }

	internal EmptyTypeBodySyntax(SyntaxTree tree, Token semicolonToken) : base(tree)
	{
		SemicolonToken = semicolonToken;
	}

	public override TextSpan Span => SemicolonToken.Span;
	public override NodeKind Kind => NodeKind.EmptyTypeBody;

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return SemicolonToken;
	}
}