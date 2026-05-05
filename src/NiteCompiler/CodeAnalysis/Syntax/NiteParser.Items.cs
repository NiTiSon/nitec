using System;
using System.Diagnostics;
using System.Linq;

namespace NiteCompiler.CodeAnalysis.Syntax;

internal partial class NiteParser
{
	private bool IsPresentedAnyModifier()
	{
		return IsPresentedAny(TokenKind.Pure, TokenKind.Static, TokenKind.Const);
	}

	private GenericParameterListSyntax ParseGenericParameterList()
	{
		Token less = MatchToken(TokenKind.Less);

		SyntaxList<GenericOrLifetimeParameterSyntax>.Builder builder = new();
		while (Current.TKind != TokenKind.Greater)
		{
			builder.Add(ParseGenericOrLifetimeParameter());

			if (Current.TKind == TokenKind.Comma)
			{
				PeekAndAdvance();
			}
			else if (Current.TKind != TokenKind.Greater)
			{
				_diagnostics.ReportExpectedToken(Current.Location, TokenKind.Comma);
			}
		}

		Token greater = MatchToken(TokenKind.Greater);

		return new GenericParameterListSyntax(_syntaxTree, less, builder.Build(_syntaxTree),  greater);
	}

	private GenericOrLifetimeParameterSyntax ParseGenericOrLifetimeParameter()
	{
		TokenKind tokenKind = Current.TKind;

		if (tokenKind == TokenKind.LifetimeIdentifier)
		{
			LifetimeSyntax lifetime = ParseLifetime();

			return new LifetimeParameterSyntax(_syntaxTree, lifetime);
		}

		if (tokenKind.IsAnyIdentifierOrKeyword) // value or type generic parameter
		{
			SimpleNameSyntax name = ParseSimpleName();

			if (name is GenericNameSyntax)
			{
				// TODO: report
			}

			return new TypeParameterSyntax(_syntaxTree, name);
		}

		throw new NotImplementedException();
	}

	private LifetimeSyntax ParseLifetime()
	{
		Debug.Assert(Current.TKind == TokenKind.LifetimeIdentifier);
		Token lifetimeToken = PeekAndAdvance();
		Debug.Assert(lifetimeToken is StringToken);

		return new LifetimeSyntax(_syntaxTree, lifetimeToken, ((StringToken)lifetimeToken).Text);
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

		if (Current.TKind == TokenKind.Interface)
		{
			Token interfaceKeyword = PeekAndAdvance();
			throw new NotImplementedException("Interfaces are not implemented yet.");
		}

		return ParseFunctionDeclaration(accessibilityToken, modifiers);
	}

	private TypeDeclarationSyntax ParseTypeDeclaration(Token accessibilityToken, SyntaxList<Token>.Builder modifiers,
		Token typeKeyword)
	{
		NameSyntax name = ParseName();

		TypeBodySyntax body = ParseTypeBody();

		return new TypeDeclarationSyntax(_syntaxTree, accessibilityToken, modifiers.Build(_syntaxTree), typeKeyword, name, body);
	}

	private TypeBodySyntax ParseTypeBody()
	{
		if (Current.TKind == TokenKind.OpenBrace)
		{
			Token openBrace = PeekAndAdvance();

			SyntaxList<MemberSyntax>.Builder membersBuilder = new();
			while (Current.TKind != TokenKind.CloseBrace &&
			       Current.TKind != TokenKind.EndOfFile)
			{
				if (IsPresentedAnyAccessibilityToken())
				{
					Token elevatedKeyword = PeekAndAdvance().ToContextualKeywordToken();
					membersBuilder.Add(ParseMember(elevatedKeyword));
				}
				else
				{
					break;
					// TODO: no break, we should try to read member,
					//	and then if member is valid -> try to construct non-error-node with diagnostic [accessibility-modifier-required]
					//	otherwise if we can't get a valid member -> try to predict incomplete member and add as error-node
				}
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

		ParameterListSyntax parameters = ParseParameterList();

		TypeClause? typeClause = null;
		if (Current.TKind == TokenKind.Retusa)
		{
			Token retusa = PeekAndAdvance();
			TypeSyntax type = ParseType();
			typeClause = new(_syntaxTree, retusa, type);
		}

		FunctionBodySyntax body = ParseFunctionBody();

		return new FunctionDeclarationSyntax(_syntaxTree, accessibilityToken, modifiers.Build(_syntaxTree), parameters, name, typeClause, body);
	}

	private ParameterSyntax ParseParameter()
	{
		SimpleNameSyntax name = ParseSimpleName();

		TypeClause? typeClause = null;
		if (Current.TKind == TokenKind.Colon)
		{
			Token colon = PeekAndAdvance();
			TypeSyntax type = ParseType();
			typeClause = new(_syntaxTree, colon, type);
		}
		else
		{
			typeClause = new(_syntaxTree, name.GetTokens().First(), name);
			_diagnostics.ReportMissingParameterTypeSpecification(name.Location);
		}

		return new ParameterSyntax(_syntaxTree, name, typeClause);
	}

	private ParameterListSyntax ParseParameterList()
	{
		Token openParen = MatchToken(TokenKind.OpenParen);

		var parameterBuilder = ParseCommaSeparatedList(TokenKind.CloseParen, ParseParameter);

		Token closeParen = MatchToken(TokenKind.CloseParen);

		return new ParameterListSyntax(_syntaxTree, openParen, parameterBuilder.Build(_syntaxTree), closeParen);
	}

	private SyntaxList<T>.Builder ParseCommaSeparatedList<T>(TokenKind terminator, Func<T> parseElement)
		where T : SyntaxNode
	{
		SyntaxList<T>.Builder elements = new();

		while (Current.TKind != terminator &&
		       Current.TKind != TokenKind.EndOfFile)
		{
			elements.Add(parseElement());

			if (Current.TKind == TokenKind.Comma)
			{
				Advance();

				if (Current.TKind == terminator) // trailing comma syntax is allowed
					break;
			}
			else
			{
				break;
			}
		}

		return elements;
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