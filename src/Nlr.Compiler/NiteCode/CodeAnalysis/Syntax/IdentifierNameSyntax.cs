using System.Collections.Generic;
using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public sealed class IdentifierNameSyntax : SimpleNameSyntax
{
	public Token Identifier { get; }

	public IdentifierNameSyntax(Token identifier)
	{
		Identifier = identifier;
	}

	public override SyntaxKind Kind => SyntaxKind.IdentifierName;

	public override IEnumerable<ISyntaxNode> GetChildren()
	{
		yield return Identifier;
	}
}