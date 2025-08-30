using System.Collections.Immutable;

namespace NiteCompiler.CodeAnalysis.Syntax;

public abstract class MemberSyntax : SyntaxNode
{
	public Token AccessibilityToken { get; }
	public ImmutableArray<Token> Modifiers { get; }
	protected MemberSyntax(Token accessibilityToken, ImmutableArray<Token> modifiers)
	{
		AccessibilityToken = accessibilityToken;
		Modifiers = modifiers;
	}
}