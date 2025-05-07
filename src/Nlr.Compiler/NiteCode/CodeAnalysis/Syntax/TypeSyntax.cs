using System.Collections.Generic;
using System.Collections.Immutable;
using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public sealed class TypeSyntax : MemberSyntax
{
	public Token TypeKeyword { get; }
	public Token Name { get; }

	public TypeSyntax(Token accessToken, ImmutableArray<Token> modifiers, Token typeKeyword, Token name) : base(accessToken, modifiers)
	{
		TypeKeyword = typeKeyword;
		Name = name;
	}

	public override SyntaxKind Kind => SyntaxKind.TypeDeclaration;
	public override IEnumerable<ISyntaxNode> GetChildren()
	{
		yield return AccessToken;
		foreach (Token modifier in Modifiers)
		{
			yield return modifier;
		}
		yield return TypeKeyword;
	}
}