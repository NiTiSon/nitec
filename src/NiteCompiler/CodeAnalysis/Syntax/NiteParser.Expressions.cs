using System;
using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed partial class NiteParser
{
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

		// Something that doesn't expand the current expression we're looking at.  Bail out and see if we
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
			return ParseExpressionContinued(ParsePrimaryOrUnaryExpression(), implPrecedence);
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

		// TODO: Add support for >>, >>>, >>>=
		Token operatorToken = ConsumeExpressionOperatorToken(operatorTokenKind);

		if (newPrecedence > operatorExpressionKind.Precedence)
		{
			const string msg = "!!! TryExpandExpression@NiteParser.Expressions.cs !!! INVALID BEHAVIOUR";
			Debug.Fail(msg);
			Console.Error.WriteLine(msg);
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

	private ExpressionSyntax ParsePrimaryExpression()
	{
		NodeKind literalType;
		if (Current.TKind == TokenKind.OpenParen)
		{
			return ParseParenthesizedExpression();
		}

		if (Current.TKind == TokenKind.IdentifierOrKeyword)
		{
			return ParseName();
		}

		if ((literalType = Current.TKind.ToLiteralExpressionKind()) != NodeKind.None)
		{
			return new LiteralExpressionSyntax(Current.Tree, PeekAndAdvance(), literalType);
		}

		throw new NotImplementedException();
	}

	private ExpressionSyntax ParsePrimaryOrUnaryExpression()
	{
		if (Current.TKind is { IsOperator: true, CanBeUnaryOperator: true })
		{
			Token operatorToken = PeekAndAdvance();
			NodeKind opKind = operatorToken.TKind.ToUnaryExpressionKind();
			ExpressionSyntax expression = ParseSubExpression(opKind.Precedence);

			return new UnaryExpressionSyntax(operatorToken.Tree, operatorToken, expression, opKind);
		}

		return ParsePrimaryExpression();
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
		if (Current.IsTypeKeyword)
		{
			return ParsePredefinedType();
		}
		else if (Current.TKind == TokenKind.IdentifierOrKeyword)
		{
			return ParseName();
		}

		throw new NotImplementedException();
	}

	private PredefinedTypeSyntax ParsePredefinedType()
	{
		return new(_syntaxTree, PeekAndAdvance());
	}

	private NameSyntax ParseName()
	{
		if (Current.TKind == TokenKind.IdentifierOrKeyword)
		{
			return ParseSimpleName();
		}

		// TODO: Qualified names
		throw new NotImplementedException();
	}

	private SimpleNameSyntax ParseSimpleName()
	{
		Token current = PeekAndAdvance();

		if (current.TKind == TokenKind.IdentifierOrKeyword)
		{
			string identifier = (current as IdentifierOrKeywordToken)!.Identifier;
			return new IdentifierNameSyntax(_syntaxTree, current, identifier);
		}

		// TODO: Escaped `identifier`
		throw new NotImplementedException();
	}

	private ModuleNameSyntax ParseModuleName()
	{
		// SimpleName (:: SimpleName)*
		SyntaxList<SimpleNameSyntax>.Builder parts = new();

		parts.Add(ParseSimpleName());

		while (Current.TKind == TokenKind.DoubleColon)
		{
			_ = MatchToken(TokenKind.DoubleColon);
			parts.Add(ParseSimpleName());
		}

		return new(_syntaxTree, parts.Build(_syntaxTree));
	}
}