using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax.Expressions.Names;

public sealed class IdentifierNameSyntax : NameSyntax
{
	public IdentifierToken Identifier { get; }

	public IdentifierNameSyntax(IdentifierToken identifier)
	{
		Identifier = identifier;
	}

	public override TextSpan Span => Identifier.Span;
	public override SyntaxKind Kind => SyntaxKind.IdentifierName;
}