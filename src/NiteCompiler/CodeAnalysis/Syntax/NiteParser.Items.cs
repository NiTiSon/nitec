using System;
using System.Diagnostics;
using System.Linq;

namespace NiteCompiler.CodeAnalysis.Syntax;

internal partial class NiteParser
{
	private bool IsPresentedAnyModifier()
	{
		return IsPresentedAny(TokenKind.Pure, TokenKind.Static, TokenKind.Const, TokenKind.Partial);
	}

	private GenericParameterListSyntax ParseGenericParameterList()
	{
		Token less = MatchToken(TokenKind.Less);

		SyntaxList<LifetimeOrGenericParameterSyntax>.Builder builder = new();
		while (Current.TKind != TokenKind.Greater &&
		       Current.TKind != TokenKind.EndOfFile)
		{
			builder.Add(ParseGenericOrLifetimeParameter());

			if (Current.TKind == TokenKind.Comma)
			{
				PeekAndAdvance();
			}
			else if (Current.TKind != TokenKind.Greater)
			{
				_diagnostics.ReportExpectedToken(Current.Location, TokenKind.Comma);
				break;
			}
		}

		Token greater = MatchToken(TokenKind.Greater);

		return new GenericParameterListSyntax(_syntaxTree, less, builder.Build(_syntaxTree),  greater);
	}

	private LifetimeOrGenericParameterSyntax ParseGenericOrLifetimeParameter()
	{
		TokenKind tokenKind = Current.TKind;

		if (tokenKind == TokenKind.LifetimeIdentifier)
		{
			return ParseLifetime();
		}

		if (tokenKind.IsAnyIdentifierOrKeyword) // value or type generic parameter
		{
			SimpleNameSyntax name = ParseSimpleName();

			if (name is GenericNameSyntax)
			{
				_diagnostics.ReportGenericsIsNotApplicableOnGenericsTypeName(name.Location);
			}

			// Value: T
			if (Current.TKind == TokenKind.Colon)
			{
				Token colon = PeekAndAdvance();

				TypeSyntax type = ParseType();
				TypeClauseSyntax typeClause = new(_syntaxTree, colon, type);

				return new GenericValueParameterSyntax(_syntaxTree, name, typeClause);
			}

			// T
			return new GenericTypeParameterSyntax(_syntaxTree, name);
		}

		SyntaxList<SyntaxNode>.Builder errorNodes = new();

		TokenKind currentKind = Current.TKind;
		_diagnostics.ReportUnexpectedToken(Current.Location, currentKind);

		while (currentKind != TokenKind.EndOfFile &&
		       currentKind != TokenKind.Greater &&
		       currentKind != TokenKind.Comma)
		{
			errorNodes.Add(PeekAndAdvance());

			currentKind = Current.TKind;
		}

		return new GenericErrorParameterSyntax(_syntaxTree, errorNodes.Build(_syntaxTree));
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
		Debug.Assert(accessibilityToken.IsKeyword || accessibilityToken.TKind == TokenKind.None);
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

		SimpleNameSyntax name = ParseSimpleName();
		GenericParameterListSyntax? genericParameterList = null;
		if (Current.TKind == TokenKind.Less)
		{
			genericParameterList = ParseGenericParameterList();
		}

		if (Current.TKind == TokenKind.Colon)
		{
			if (genericParameterList != null)
			{
				_diagnostics.ReportGenericsNotApplicableOnThisItem(genericParameterList.Location);
			}

			return ParseFieldDeclaration(accessibilityToken, modifiers, name);
		}

		return ParseFunctionDeclaration(accessibilityToken, modifiers, name, genericParameterList);
	}

	private FieldDeclarationSyntax ParseFieldDeclaration(Token accessibilityToken, SyntaxList<Token>.Builder modifiers, SimpleNameSyntax name)
	{
		Token colon = MatchToken(TokenKind.Colon);
		TypeSyntax type = ParseType();
		Token semicolon = MatchToken(TokenKind.Semicolon);
		TypeClauseSyntax typeClause = new(_syntaxTree, colon, type);
		return new FieldDeclarationSyntax(_syntaxTree, accessibilityToken, modifiers.Build(_syntaxTree), name, typeClause, semicolon);
	}

	private TypeDeclarationSyntax ParseTypeDeclaration(Token accessibilityToken, SyntaxList<Token>.Builder modifiers,
		Token typeKeyword)
	{
		NameSyntax name = ParseInlineName();
		GenericParameterListSyntax? genericParameterList = null;
		if (Current.TKind == TokenKind.Less)
		{
			genericParameterList = ParseGenericParameterList();
		}

		TypeBodySyntax body = ParseTypeBody();

		return new TypeDeclarationSyntax(_syntaxTree, accessibilityToken, modifiers.Build(_syntaxTree), typeKeyword, name, genericParameterList, body);
	}

	private NameSyntax ParseInlineName()
	{
		ResetPoint rp = GetResetPoint();
		// (SimpleName GenericArgumentList? '::')* SimpleName
		SimpleNameSyntax first = ParseSimpleName();

		if (Current.TKind != TokenKind.Less &&
		    Current.TKind != TokenKind.DoubleColon)
		{
			ReleaseResetPoint(rp);
			return first;
		}

		NameSyntax left = first;

		while (true)
		{
			GenericParameterListSyntax? genericParameters = null;

			UpdateResetPoint(ref rp); // in case we reach the end on inline-name, we should come back right before them
			if (Current.TKind == TokenKind.Less)
			{
				genericParameters = ParseGenericParameterList();
			}

			if (Current.TKind != TokenKind.DoubleColon)
			{
				if (genericParameters != null)
				{
					Reset(rp);
				}
				break;
			}

			Token doubleColon = PeekAndAdvance();

			SimpleNameSyntax right = ParseSimpleName();

			left = new InlineNameSyntax(_syntaxTree, left, doubleColon, right, genericParameters);

			if (Current.TKind != TokenKind.Less &&
			    Current.TKind != TokenKind.DoubleColon)
			{
				break;
			}
		}

		ReleaseResetPoint(rp);

		return left;
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
					Token missingToken = new Token.Default(_syntaxTree, TokenKind.None, Current.Span,
						SyntaxList<Trivia>.GetEmpty(_syntaxTree), SyntaxList<Trivia>.GetEmpty(_syntaxTree));
					_diagnostics.ReportAccessibilityModifierRequiredBeforeMemberDeclaration(Current.Location);
					membersBuilder.Add(ParseMember(missingToken));
				}
			}

			Token closeBrace = MatchToken(TokenKind.CloseBrace);

			return new MembersTypeBodySyntax(_syntaxTree, openBrace, membersBuilder.Build(_syntaxTree), closeBrace);
		}
		else if (Current.TKind == TokenKind.Semicolon)
		{
			return new EmptyTypeBodySyntax(_syntaxTree, PeekAndAdvance());
		}

		SyntaxList<SyntaxNode>.Builder errorNodes = new();

		// if met open brace, probably within the real one body -> definitely not within empty body syntax
		bool metOpenBrace = false;

		TokenKind currentKind = Current.TKind;
		_diagnostics.ReportUnexpectedToken(Current.Location, currentKind);
		while (currentKind != TokenKind.EndOfFile &&
		       currentKind != TokenKind.CloseBrace &&
		       (currentKind != TokenKind.Semicolon && !metOpenBrace))
		{
			if (currentKind == TokenKind.OpenBrace)
			{
				metOpenBrace = true;
			}

			errorNodes.Add(PeekAndAdvance());

			currentKind = Current.TKind;
		}

		return new ErrorTypeBodySyntax(_syntaxTree, errorNodes.Build(_syntaxTree));
	}

	private FunctionDeclarationSyntax ParseFunctionDeclaration(Token accessibilityToken, SyntaxList<Token>.Builder modifiers,
		SimpleNameSyntax name, GenericParameterListSyntax? genericParameterList)
	{
		ParameterListSyntax parameters = ParseParameterList();

		TypeClauseSyntax? typeClause = null;
		if (Current.TKind == TokenKind.Retusa)
		{
			Token retusa = PeekAndAdvance();
			TypeSyntax type = ParseType();
			typeClause = new(_syntaxTree, retusa, type);
		}

		SyntaxList<LifetimeOrGenericConstraintClauseSyntax>? constraintClauses = null;
		if (Current.TKind.IsAnyIdentifierOrKeyword &&
		    Current.TKind.ToContextualKeyword() != TokenKind.Where)
		{
			constraintClauses = ParseConstraintClauses();
		}


		FunctionBodySyntax body = ParseFunctionBody();

		return new FunctionDeclarationSyntax(_syntaxTree,
			accessibilityToken, modifiers.Build(_syntaxTree), name, genericParameterList, parameters, typeClause,
			constraintClauses, body);
	}

	private ParameterSyntax ParseParameter()
	{
		SimpleNameSyntax name = ParseSimpleName();

		TypeClauseSyntax? typeClause = null;
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
		if (Current.TKind == TokenKind.Semicolon)
		{
			return new EmptyFunctionBodySyntax(_syntaxTree, PeekAndAdvance());
		}

		SyntaxList<Token>.Builder erroredNodes = new();
		TokenKind currentKind = Current.TKind;
		while (currentKind != TokenKind.EndOfFile &&
		       currentKind != TokenKind.CloseBrace)
		{
			erroredNodes.Add(PeekAndAdvance());

			currentKind = Current.TKind;
		}

		var nodes = erroredNodes.Build(_syntaxTree);
		_diagnostics.ReportExpectedToken((nodes.Count > 0 ? nodes[0] : Current).Location, TokenKind.OpenBrace);
		return new ErrorFunctionBodySyntax(_syntaxTree,  nodes);
	}

	private SyntaxList<LifetimeOrGenericConstraintClauseSyntax> ParseConstraintClauses()
	{
		Debug.Assert(Current.GetContextualKeyword() == TokenKind.Where);
		var builder = new SyntaxList<LifetimeOrGenericConstraintClauseSyntax>.Builder();

		TokenKind currentKind = Current.TKind;
		while (currentKind != TokenKind.EndOfFile &&
		       currentKind != TokenKind.OpenBrace &&
		       currentKind != TokenKind.Semicolon)
		{
			if (currentKind.IsAnyIdentifierOrKeyword &&
			    currentKind.GetContextualKeyword() == TokenKind.Where)
			{
				var constraint = ParseConstraintClause();

				builder.Add(constraint);
			}
			else
			{
				Token current = PeekAndAdvance();
				_diagnostics.ReportUnexpectedToken(current.Location, current.TKind);
			}

			if (Current.TKind == TokenKind.Comma)
			{
				Token comma = PeekAndAdvance();
				_diagnostics.ReportUnexpectedToken(comma.Location, comma.TKind);
			}

			currentKind = Current.TKind;
		}

		return builder.Build(_syntaxTree);
	}

	private LifetimeOrGenericConstraintClauseSyntax ParseConstraintClause()
	{
		if (Current.TKind.IsAnyIdentifierOrKeyword &&
		    Current.TKind.GetContextualKeyword() == TokenKind.Where)
		{
			Token whereKeyword = PeekAndAdvance();

			if (Current.TKind.IsAnyIdentifierOrKeyword) // definitely a type or value constraint
			{
				throw new NotImplementedException("Type constraints are not implemented.");
			}
			else if (Current.TKind == TokenKind.LifetimeIdentifier)
			{
				LifetimeSyntax lifetime = ParseLifetime();
				Token colonToken = MatchToken(TokenKind.Colon);
				SyntaxList<ConstraintSyntax> constraints = ParseConstraints();

				return new LifetimeConstraintClauseSyntax(_syntaxTree,  whereKeyword, lifetime, colonToken, constraints);
			}
			else // if it's not a type nor value nor lifetime then an error
			{
				SyntaxList<SyntaxNode>.Builder erroredNode = new();

				TokenKind currentKind = Current.TKind;
				while (currentKind != TokenKind.EndOfFile &&
				       currentKind != TokenKind.OpenBrace && // we don't want to consume the whole method as errored nodes
				       currentKind != TokenKind.Semicolon)
				{
					if (currentKind.IsAnyIdentifierOrKeyword &&
					    currentKind.GetContextualKeyword() == TokenKind.Where)
					{
						break;
					}

					erroredNode.Add(PeekAndAdvance());
					currentKind = Current.TKind;
				}

				// TODO: report
				return new ErrorConstraintClauseSyntax(_syntaxTree, erroredNode.Build(_syntaxTree));
			}
		}
		else // otherwise try to recover
		{
			SyntaxList<SyntaxNode>.Builder erroredNode = new();

			TokenKind currentKind = Current.TKind;
			while (currentKind != TokenKind.EndOfFile &&
			       currentKind != TokenKind.OpenBrace && // we don't want to consume the whole method as errored nodes
			       currentKind != TokenKind.Semicolon)
			{
				if (currentKind.IsAnyIdentifierOrKeyword &&
				    currentKind.GetContextualKeyword() == TokenKind.Where)
				{
					goto FOUND_WHERE;
				}

				erroredNode.Add(PeekAndAdvance());
				currentKind = Current.TKind;
			}

			// TODO: report
			return new ErrorConstraintClauseSyntax(_syntaxTree, erroredNode.Build(_syntaxTree));

			FOUND_WHERE:
			// TODO: report (if we here, there at least one errored node)
			return ParseConstraintClause();
		}
	}

	private SyntaxList<ConstraintSyntax> ParseConstraints()
	{
		// constraints are plus separated
		SyntaxList<ConstraintSyntax>.Builder constraints = new();
		TokenKind currentKind = Current.TKind;
		while (currentKind != TokenKind.EndOfFile &&
		       currentKind != TokenKind.OpenBrace &&
		       currentKind != TokenKind.Semicolon)
		{
			if (Current.TKind.IsAnyIdentifierOrKeyword &&
			    Current.TKind.GetContextualKeyword() == TokenKind.Where)
			{
				break;
			}

			constraints.Add(ParseConstraint());

			if (Current.TKind == TokenKind.Plus)
			{
				Advance();
			}
			else if (Current.TKind == TokenKind.Comma) // comma are wrong token, but understandable: should report
			{
				Token comma = PeekAndAdvance();
				_diagnostics.ReportExpectedToken(comma.Location, TokenKind.Plus);
			}

			currentKind = Current.TKind;
		}

		// empty constrains are reported during declaration phase
		return constraints.Build(_syntaxTree);
	}

	private ConstraintSyntax ParseConstraint()
	{
		if (Current.TKind == TokenKind.LifetimeIdentifier)
		{
			return new LifetimeConstraintSyntax(_syntaxTree, ParseLifetime());
		}
		else
		{
			throw new NotImplementedException("Other that lifetime constraints are not implemented.");
		}
	}
}