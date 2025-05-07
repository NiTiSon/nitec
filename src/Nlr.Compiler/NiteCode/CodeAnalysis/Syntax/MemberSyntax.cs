using System.Collections.Generic;
using System.Collections.Immutable;
using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public abstract class MemberSyntax : ISyntaxNode
{
	public Token AccessToken { get; }
	public ImmutableArray<Token> Modifiers { get; }

	protected MemberSyntax(Token accessToken, ImmutableArray<Token> modifiers)
	{
		Modifiers = modifiers;
		AccessToken = accessToken;
	}
	
	public abstract SyntaxKind Kind { get; }
	public abstract IEnumerable<ISyntaxNode> GetChildren();
}