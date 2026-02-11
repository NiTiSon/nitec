using System;

namespace NiteCompiler.CodeAnalysis.Syntax;

public partial class NiteParser
{
	private bool IsPresentedAnyModifier()
	{
		return IsPresentedAny(TokenKind.Pure, TokenKind.Static, TokenKind.Const);
	}

	public ItemSyntax ParseItem(Token accessibilityToken)
	{
		SyntaxList<Token>.Builder modifiers = new();
		while (IsPresentedAnyModifier())
		{
			modifiers.Add(PeekAndAdvance());
		}

		if (Current.TKind == TokenKind.Type)
		{
			throw new NotImplementedException("Types not yet implemented");
		}
		else
		{
			return ParseFunction(accessibilityToken, modifiers);
		}
	}

	private ItemSyntax ParseFunction(Token accessibilityToken, SyntaxList<Token>.Builder modifiers)
	{
		SimpleNameSyntax name = ParseSimpleName();

		Token openParenToken = MatchToken(TokenKind.OpenParen);
		Token closeParenToken = MatchToken(TokenKind.CloseParen);

		FunctionBodySyntax body = ParseFunctionBody();

		return new FunctionSyntax(accessibilityToken.Tree, accessibilityToken, modifiers.Build(accessibilityToken.Tree), name, body);
	}

	private FunctionBodySyntax ParseFunctionBody()
	{
		if (Current.TKind == TokenKind.OpenBrace)
		{
			BlockStatementSyntax block = ParseBlockStatement();
			return new BlockFunctionBodySyntax(_syntaxTree, block);
		}
		else if (Current.TKind == TokenKind.Semicolon)
		{
			return new EmptyFunctionBodySyntax(_syntaxTree, PeekAndAdvance());
		}

		throw new NotImplementedException("");
	}
}