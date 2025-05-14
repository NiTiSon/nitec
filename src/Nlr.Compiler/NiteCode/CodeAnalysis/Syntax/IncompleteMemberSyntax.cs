using System.Collections.Generic;
using System.Collections.Immutable;
using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public sealed class IncompleteMemberSyntax : MemberSyntax
{
	public IncompleteMemberSyntax(Token accessToken, ImmutableArray<Token> modifiers) : base(accessToken, modifiers) {}

	public override SyntaxKind Kind => SyntaxKind.IncompleteMemberDeclaration;
	public override IEnumerable<ISyntaxNode> GetChildren()
	{
		yield break;
	}
}