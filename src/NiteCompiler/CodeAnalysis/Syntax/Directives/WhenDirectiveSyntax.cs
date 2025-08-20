using System;
using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax.Directives;

public sealed class WhenDirectiveSyntax : AttributeDirectiveSyntax
{
	public WhenDirectiveSyntax(Token hash, object todo) : base(hash)
	{
		throw new NotImplementedException();
	}

	public override TextSpan Span => throw new NotImplementedException();
	public override SyntaxKind Kind => throw new NotImplementedException();
	public override IEnumerable<SyntaxNode> GetChildren()
	{
		throw new NotImplementedException();
	}
}