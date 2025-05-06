using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public sealed class IdentifierNameSyntax : SimpleMemberNameSyntax
{
	public Token Identifier { get; }

	public IdentifierNameSyntax(Token identifier)
	{
		Identifier = identifier;
	}

	public override SyntaxKind Kind => SyntaxKind.IdentifierName;
}