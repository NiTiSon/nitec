using System.Collections.Generic;
using System.Collections.Immutable;
using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public sealed class FunctionSyntax : MemberSyntax
{
	public Token Name { get; }
	public BlockSyntax Body { get; }
	public FunctionSyntax(Token accessToken, ImmutableArray<Token> modifiers, Token name, BlockSyntax body) : base(accessToken, modifiers)
	{
		Name = name;
		Body = body;
	}
	public override SyntaxKind Kind => SyntaxKind.FunctionDeclaration;

	public override IEnumerable<ISyntaxNode> GetChildren()
	{
		yield return Name;
		yield return Body;
	}
}