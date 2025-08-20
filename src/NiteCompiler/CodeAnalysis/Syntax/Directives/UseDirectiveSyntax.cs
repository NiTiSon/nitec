using System;
using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax.Directives;

public sealed class UseDirectiveSyntax : SyntaxNode
{
	public Token UseKeyword { get; }

	public UseDirectiveSyntax(Token useKeyword, object? todo = null)
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