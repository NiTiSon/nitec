using System;
using System.Diagnostics;
using System.Text;
using NiteCompiler.CodeAnalysis.Text;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Syntax;

internal sealed partial class NiteParser
{
	[Flags]
	private enum NameOptions
	{
		None = 0,
		InExpression = 1 << 0,
		InItemName = 1 << 1,
	}

	private (TokenKind operatorTokenKind, NodeKind operatorExpressionKind) GetExpressionOperatorTokenKindAndExpressionKind()
	{
		// If the set of expression continuations is updated here, please review ParseStatementAttributeDeclarations
		// to see if it may need a similar look-ahead check to determine if something is a collection expression versus
		// an attribute.

		Token token1 = Current;
		TokenKind token1Kind = token1.TKind;
		Token token2 = Peek(1);

		// check for >>, >>=, >>> or >>>=
		//
		// In all those cases, update token1Kind to be the merged token kind.  It will then be handled by the code below.
		if (token1Kind == TokenKind.Greater
			&& (token2.TKind == TokenKind.Greater || token2.TKind == TokenKind.GreaterOrEquals)
			&& token1.IsBefore(token2)) // check to see if they really are adjacent
		{
			if (token2.TKind == TokenKind.Greater)
			{
				Token token3 = Peek(2);
				if ((token3.TKind == TokenKind.Greater || token3.TKind == TokenKind.GreaterOrEquals)
					&& token2.IsBefore(token3)) // check to see if they really are adjacent
				{
					// >>>  or  >>>=
					token1Kind = token3.TKind == TokenKind.Greater
						? TokenKind.RightUnsignedShift
						: TokenKind.RightUnsignedShiftAssignment;
				}
				else
				{
					// >>
					token1Kind = TokenKind.RightArithmeticShift;
				}
			}
			else
			{
				// >>=
				token1Kind = TokenKind.RightArithmeticShiftAssignment;
			}
		}

		if (token1Kind is { IsOperator: true, IsAssignmentOperator: true })
		{
			return (token1Kind, token1Kind.ToAssignmentExpressionKind());
		}

		if (token1Kind is { IsOperator: true, CanBeBinaryOperator: true })
		{
			return (token1Kind, token1Kind.ToBinaryExpressionKind());
		}

		// something that doesn't expand the current expression we're looking at.  Bail out and see if we
		// can end with a conditional expression.
		return (TokenKind.None, NodeKind.None);
	}

	private Token ConsumeExpressionOperatorToken(TokenKind operatorTokenKind)
	{
		if (operatorTokenKind == TokenKind.RightArithmeticShift ||
		    operatorTokenKind == TokenKind.RightArithmeticShiftAssignment)
		{
			// >> and >>=
			Token token1 = PeekAndAdvance();
			Token token2 = PeekAndAdvance();

			return CombineTokens(token1, token2, operatorTokenKind);
		}

		if (operatorTokenKind == TokenKind.RightUnsignedShift ||
		    operatorTokenKind == TokenKind.RightUnsignedShiftAssignment)
		{
			// >>> and >>>=
			Token token1 = PeekAndAdvance();
			_ = PeekAndAdvance();
			Token token3 = PeekAndAdvance();

			return CombineTokens(token1, token3, operatorTokenKind);
		}

		return PeekAndAdvance();

		static Token CombineTokens(Token leftMost, Token rightMost, TokenKind operatorTokenKind)
		{
			return new Token.Default(
				leftMost.Tree,
				operatorTokenKind,
				TextSpan.FromBounds(leftMost.Span, rightMost.Span),
				leftMost.LeadingTrivia,
				rightMost.TrailingTrivia
			);
		}
	}

	private ExpressionSyntax ParseExpression()
	{
		return ParseSubExpression(Precedence.Expression);
	}

	private ExpressionSyntax ParseSubExpression(Precedence precedence)
	{
		ExpressionSyntax result = Impl(precedence);

		#if DEBUG
		_ = result.Kind.Precedence;
		#endif

		return result;

		ExpressionSyntax Impl(Precedence implPrecedence)
		{
			return ParseExpressionContinued(ParsePrimaryOrUnaryExpression(implPrecedence), implPrecedence);
		}
	}

	private ExpressionSyntax ParseExpressionContinued(ExpressionSyntax unaryOrPrimaryExpression, Precedence precedence)
	{
		ExpressionSyntax currentExpression = unaryOrPrimaryExpression;

		while (TryExpandExpression(currentExpression, precedence) is { } expandedExpression)
			currentExpression = expandedExpression;

		return currentExpression;
	}

	private ExpressionSyntax? TryExpandExpression(ExpressionSyntax leftOperand, Precedence precedence)
	{
		(TokenKind operatorTokenKind, NodeKind operatorExpressionKind) = GetExpressionOperatorTokenKindAndExpressionKind();

		if (operatorTokenKind == TokenKind.None)
			return null;

		Precedence newPrecedence = operatorExpressionKind.Precedence;

		#if TRACE && false
		Console.WriteLine($"opKind={operatorExpressionKind} newPrec={(int)newPrecedence} prec={(int)precedence} isRight={operatorExpressionKind.IsRightAssociative}");
		#endif

		if (newPrecedence < precedence)
			return null;

		if ((newPrecedence == precedence) && !operatorExpressionKind.IsRightAssociative)
			return null;

		Token operatorToken = ConsumeExpressionOperatorToken(operatorTokenKind);

		if (newPrecedence > operatorExpressionKind.Precedence)
		{
			const string msg = "!!! TryExpandExpression@NiteParser.Expressions.cs !!! INVALID BEHAVIOUR";
			Debug.Fail(msg);
		}

		if (operatorToken.TKind.IsAssignmentOperator)
		{
			return ParseAssignmentExpression(operatorExpressionKind, leftOperand, operatorToken);
		}
		if (operatorToken.TKind.CanBeBinaryOperator)
		{
			return new BinaryExpressionSyntax(leftOperand.Tree, leftOperand, operatorToken, ParseSubExpression(newPrecedence), operatorExpressionKind);
		}

		throw new UnreachableException();
	}

	private AssignmentExpressionSyntax ParseAssignmentExpression(NodeKind operatorExpressionKind,
		ExpressionSyntax leftOperand, Token operatorToken)
	{
		ExpressionSyntax rhs = ParseSubExpression(Precedence.Assignment);

		return new(leftOperand.Tree, leftOperand, operatorToken, rhs, operatorExpressionKind);
	}

	private ExpressionSyntax ParsePrimaryExpression(Precedence precedence)
	{
		// primary expressions:
		// x, [...], x.y, x(...), x[...]
		return ParsePostFixExpression(ParsePrimaryExpressionWithoutPostfix(precedence));

		ExpressionSyntax ParsePrimaryExpressionWithoutPostfix(Precedence precedence)
		{
			TokenKind tokenKind = Current.TKind;

			// TODO: default, sizeof, etc.
			if (tokenKind == TokenKind.OpenParen)
			{
				return ParseParenthesizedExpression();
			}

			if (tokenKind.IsAnyIdentifierOrKeyword)
			{
				return ParsePathName();
			}

			if (tokenKind == TokenKind.True ||
			    tokenKind == TokenKind.False ||
			    tokenKind == TokenKind.NumberLiteral ||
			    tokenKind == TokenKind.CharacterLiteral ||
			    tokenKind == TokenKind.StringLiteral)
			{
				Token current = PeekAndAdvance();
				if (tokenKind == TokenKind.CharacterLiteral)
				{
					ValidateCharacterLiteral(current);
				}
				return new LiteralExpressionSyntax(_syntaxTree, current, current.TKind.ToLiteralExpressionKind());
			}

			if (tokenKind == TokenKind.OpenBracket)
			{
				return ParseCollectionExpression();
			}

			throw new NotImplementedException();
		}

		ExpressionSyntax ParsePostFixExpression(ExpressionSyntax expression)
		{
			while (true) // postfix
			{
				TokenKind tokenKind = Current.TKind;
				if (tokenKind == TokenKind.OpenParen)
				{
					expression = new InvocationExpressionSyntax(_syntaxTree, expression, ParseParenthesizedArgumentList());
				}
				else if (tokenKind == TokenKind.OpenBracket)
				{
					expression = new IndexationExpressionSyntax(_syntaxTree, expression, ParseBracketedArgumentList());
				}
				else
				{
					return expression;
				}
			}
		}
	}

	private void ValidateCharacterLiteral(Token token)
	{
		StringToken? stringToken = token as StringToken;
		if (stringToken is null)
		{
			return;
		}

		string text = stringToken.Text;
		Span<Rune> runes = stackalloc Rune[2];
		int runeCount = 0;
		foreach (Rune rune in text.EnumerateRunes())
		{
			if (runeCount >= runes.Length)
			{
				break;
			}
			runes[runeCount++] = rune;
		}

		if (runeCount != 1)
		{
			_diagnostics.ReportInvalidCharacterLiteral(token.Span.Contextualize(_syntaxTree));
			return;
		}

		Rune singleRune = runes[0];
		int requiredBytes;
		string encodingName;
		switch (stringToken.LiteralType)
		{
			case StringLiteralType.None:
			case StringLiteralType.Unicode8:
				requiredBytes = Encoding.UTF8.GetByteCount(singleRune.ToString());
				encodingName = "utf8";
				if (requiredBytes != 1)
				{
					_diagnostics.ReportInvalidCharacterLiteralEncoding(token.Span.Contextualize(_syntaxTree), encodingName, requiredBytes);
				}
				break;
			case StringLiteralType.Unicode16:
				requiredBytes = Encoding.Unicode.GetByteCount(singleRune.ToString());
				encodingName = "utf16";
				if (requiredBytes != 2)
				{
					_diagnostics.ReportInvalidCharacterLiteralEncoding(token.Span.Contextualize(_syntaxTree), encodingName, requiredBytes);
				}
				break;
			case StringLiteralType.Unicode32:
				requiredBytes = Encoding.UTF32.GetByteCount(singleRune.ToString());
				encodingName = "utf32";
				if (requiredBytes != 4)
				{
					_diagnostics.ReportInvalidCharacterLiteralEncoding(token.Span.Contextualize(_syntaxTree), encodingName, requiredBytes);
				}
				break;
			case StringLiteralType.Os:
				break;
		}
	}

	private ArgumentListSyntax ParseParenthesizedArgumentList()
	{
		Token openParen = MatchToken(TokenKind.OpenParen);
		SyntaxList<ExpressionSyntax>.Builder arguments = new();
		while (true)
		{
			if (Current.TKind == TokenKind.EndOfFile || Current.TKind == TokenKind.CloseParen) break;

			arguments.Add(ParseExpression());
			if (Current.TKind == TokenKind.Comma)
			{
				Advance();
			}

			if (Current.TKind == TokenKind.CloseParen) break;
		}
		Token closeParen = MatchToken(TokenKind.CloseParen);
		return new(_syntaxTree, openParen, arguments.Build(_syntaxTree), closeParen);
	}

	private BracketedArgumentListSyntax ParseBracketedArgumentList()
	{
		Token openBracket = MatchToken(TokenKind.OpenBracket);
		SyntaxList<ExpressionSyntax>.Builder arguments = new();
		while (true)
		{
			if (Current.TKind == TokenKind.EndOfFile) break;
			if (Current.TKind == TokenKind.CloseBracket) break;

			arguments.Add(ParseExpression());
			if (Current.TKind == TokenKind.Comma)
			{
				Advance();
			}

			if (Current.TKind == TokenKind.CloseBracket) break;
		}
		Token closeBracket = MatchToken(TokenKind.CloseBracket);
		return new(_syntaxTree, openBracket, arguments.Build(_syntaxTree), closeBracket);
	}

	private ExpressionSyntax ParseCollectionExpression()
	{
		throw new NotImplementedException();
	}

	private ExpressionSyntax ParsePrimaryOrUnaryExpression(Precedence precedence)
	{
		if (Current.TKind == TokenKind.Ampersand)
		{
			Token operatorToken = PeekAndAdvance();
			ExpressionSyntax expression = ParseSubExpression(NodeKind.AddressOfExpression.Precedence);
			return new UnaryExpressionSyntax(operatorToken.Tree, operatorToken, expression, NodeKind.AddressOfExpression);
		}

		if (Current.TKind == TokenKind.Asterisk)
		{
			Token operatorToken = PeekAndAdvance();
			ExpressionSyntax expression = ParseSubExpression(NodeKind.DereferencingExpression.Precedence);
			return new UnaryExpressionSyntax(operatorToken.Tree, operatorToken, expression, NodeKind.DereferencingExpression);
		}

		if (Current.TKind is { IsOperator: true, CanBeUnaryOperator: true })
		{
			Token operatorToken = PeekAndAdvance();
			NodeKind opKind = operatorToken.TKind.ToUnaryExpressionKind();
			ExpressionSyntax expression = ParseSubExpression(opKind.Precedence);

			return new UnaryExpressionSyntax(operatorToken.Tree, operatorToken, expression, opKind);
		}

		return ParsePrimaryExpression(precedence);
	}

	private ParenthesizedExpressionSyntax ParseParenthesizedExpression()
	{
		Token openParen = MatchToken(TokenKind.OpenParen);

		ExpressionSyntax expression = ParseExpression();

		Token closeParen = MatchToken(TokenKind.CloseParen);

		return new(openParen.Tree, openParen, expression, closeParen);
	}

	private TypeSyntax ParseType()
	{
		if (Current.IsPredefinedTypeKeyword)
		{
			return ParsePredefinedType();
		}

		TokenKind currentKind = Current.TKind;
		if (currentKind == TokenKind.IdentifierOrKeyword ||
		    currentKind == TokenKind.EscapedIdentifier)
		{
			return ParseName();
		}

		if (currentKind == TokenKind.Ampersand)
		{
			Token ampersand = PeekAndAdvance();
			return ParseReferenceType(ampersand);
		}

		if (currentKind == TokenKind.DoubleAmpersand)
		{
			// TODO: fix #3
			Token combinedToken = PeekAndAdvance();
			Token firstAmpersand = new Token.Default(
				_syntaxTree,
				TokenKind.Ampersand,
				combinedToken.Span.SubSpan(0, 1),
				combinedToken.LeadingTrivia,
				SyntaxList<Trivia>.GetEmpty(_syntaxTree));
			Token secondAmpersand = new Token.Default(
				_syntaxTree,
				TokenKind.Ampersand,
				combinedToken.Span.SubSpan(1, 1),
				SyntaxList<Trivia>.GetEmpty(_syntaxTree),
				combinedToken.TrailingTrivia);

			TypeSyntax type = ParseType();
			type = new ReferenceTypeSyntax(_syntaxTree, secondAmpersand, null, null, null, type);
			type = new ReferenceTypeSyntax(_syntaxTree, firstAmpersand, null, null, null, type);

			return type;
		}

		throw new NotImplementedException();
	}

	private PredefinedTypeSyntax ParsePredefinedType()
	{
		return new(_syntaxTree, PeekAndAdvance());
	}

	private ReferenceTypeSyntax ParseReferenceType(Token ampersand)
	{
		Debug.Assert(ampersand.TKind == TokenKind.Ampersand);
		// TODO: improve
		// the syntaxes such
		// &?'a
		// &?const?
		// are definitely wrong, and we definitely should report, but it's better if we still want to recover the valid syntax

		LifetimeSyntax? lifetime = null;
		if (Current.TKind == TokenKind.LifetimeIdentifier)
		{
			lifetime = ParseLifetime();
		}

		Token? constKeyword = null;
		if (Current.TKind == TokenKind.Const)
		{
			constKeyword = PeekAndAdvance();
		}

		Token? questionToken = null;
		if (Current.TKind == TokenKind.QuestionSign)
		{
			questionToken = PeekAndAdvance();
		}

		TypeSyntax elementType = ParseType();

		return new ReferenceTypeSyntax(_syntaxTree, ampersand, lifetime, constKeyword, questionToken, elementType);
	}

	private NameSyntax ParseName()
	{
		return ParsePathName();
	}

	private SimpleNameSyntax ParseSimpleName(NameOptions options = NameOptions.None)
	{
		Token current = PeekAndAdvance();
		Debug.Assert(current.TKind.IsAnyIdentifierOrKeyword);

		Token identifier;
		string identifierText;
		if (current.TKind == TokenKind.IdentifierOrKeyword)
		{
			identifier = current;
			identifierText = (current as IdentifierOrKeywordToken)!.Identifier;
		}
		else if (current.TKind == TokenKind.EscapedIdentifier)
		{
			identifier = current;
			identifierText = (current as StringToken)!.Text;
		}
		else
		{
			throw new UnreachableException($"ParseSimpleName({current.TKind})");
		}

		if (Current.TKind == TokenKind.Less && options.HasFlag(NameOptions.InExpression))
		{
			Token openToken = PeekAndAdvance();

			SyntaxList<ExpressionSyntax>.Builder arguments = ParseCommaSeparatedList(TokenKind.Greater, ParseExpression);

			Token closeToken = MatchToken(TokenKind.Greater);

			return new GenericNameSyntax(_syntaxTree, identifier, identifierText, openToken, arguments.Build(_syntaxTree), closeToken);
		}
		else
		{
			return new IdentifierNameSyntax(_syntaxTree, identifier, identifierText);
		}
	}

	private NameSyntax ParseModuleName()
	{
		// SimpleName (:: SimpleName)*
		NameSyntax result = ParsePathName();
		NameSyntax current = result;
		while (true)
		{
			VerifyModulePart(current.UnqualifiedName);

			if (current is PathNameSyntax path)
			{
				current = path.Left;
			}
			else break;
		}

		return result;

		void VerifyModulePart(SimpleNameSyntax nameSyntax)
		{
			if (nameSyntax is not IdentifierNameSyntax)
			{
				Location? location = (nameSyntax as GenericNameSyntax)?.GenericArguments.Location;
				_diagnostics.ReportGenericsIsNotApplicableOnModuleName(location ?? nameSyntax.Location);
			}
		}
	}

	private NameSyntax ParsePathName()
	{
		NameSyntax result = ParseSimpleName();

		while (Current.TKind == TokenKind.DoubleColon)
		{
			Token doubleColon = PeekAndAdvance();
			SimpleNameSyntax right = ParseSimpleName();
			result = new PathNameSyntax(_syntaxTree, result, doubleColon, right);
		}

		return result;
	}
}