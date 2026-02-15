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

		TypeClause? typeClause = null;
		if (Current.TKind == TokenKind.Retusa)
		{
			Token retusa = PeekAndAdvance();
			TypeSyntax type = ParseType();
			typeClause = new(_syntaxTree, retusa, type);
		}

		FunctionBodySyntax body = ParseFunctionBody();

		return new FunctionDeclarationSyntax(_syntaxTree, accessibilityToken, modifiers.Build(_syntaxTree), name, typeClause, body);
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