using System;
using System.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Syntax;

public partial class NiteParser
{
	private bool IsPresentedAnyModifier()
	{
		return IsPresentedAny(TokenKind.Pure, TokenKind.Static, TokenKind.Const);
	}

	public MemberSyntax ParseMember(Token accessibilityToken)
	{
		Debug.Assert(accessibilityToken.IsKeyword);
		SyntaxList<Token>.Builder modifiers = new();
		while (IsPresentedAnyModifier())
		{
			modifiers.Add(PeekAndAdvance());
		}

		if (Current.TKind == TokenKind.Type)
		{
			Token typeKeyword = PeekAndAdvance();
			return ParseTypeDeclaration(accessibilityToken, modifiers, typeKeyword);
		}
		else
		{
			return ParseFunctionDeclaration(accessibilityToken, modifiers);
		}
	}

	private TypeDeclarationSyntax ParseTypeDeclaration(Token accessibilityToken, SyntaxList<Token>.Builder modifiers,
		Token typeKeyword)
	{
		SimpleNameSyntax name = ParseSimpleName();

		TypeBodySyntax body = ParseTypeBody();

		return new TypeDeclarationSyntax(_syntaxTree, accessibilityToken, modifiers.Build(_syntaxTree), typeKeyword, name, body);
	}

	private TypeBodySyntax ParseTypeBody()
	{
		if (Current.TKind == TokenKind.OpenBrace)
		{
			Token openBrace = PeekAndAdvance();

			SyntaxList<MemberSyntax>.Builder membersBuilder = new();
			if (IsPresentedAnyAccessibilityToken())
			{
				Token elevatedKeyword = PeekAndAdvance().ToContextualKeywordToken();
				membersBuilder.Add(ParseMember(elevatedKeyword));
			}

			Token closeBrace = MatchToken(TokenKind.CloseBrace);

			return new MembersTypeBodySyntax(_syntaxTree, openBrace, membersBuilder.Build(_syntaxTree), closeBrace);
		}
		else if (Current.TKind == TokenKind.Semicolon)
		{
			return new EmptyTypeBodySyntax(_syntaxTree, PeekAndAdvance());
		}

		throw new UnreachableException();
	}

	private FunctionDeclarationSyntax ParseFunctionDeclaration(Token accessibilityToken, SyntaxList<Token>.Builder modifiers)
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

		throw new UnreachableException();
	}
}