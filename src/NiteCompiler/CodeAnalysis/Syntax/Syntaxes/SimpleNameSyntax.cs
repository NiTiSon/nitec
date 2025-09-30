using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class SimpleNameSyntax : NameSyntax
{
	public IdentifierToken Identifier { get; }

	public SimpleNameSyntax(IdentifierToken identifier)
	{
		Identifier = identifier;
	}

	public override TextSpan Span => Identifier.Span;
	public override SyntaxKind Kind => SyntaxKind.SimpleName;

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Identifier;
	}
}