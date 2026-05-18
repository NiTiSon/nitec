using System;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

internal sealed partial class NiteParser
{
	private StatementSyntax ParseStatement()
	{
		EnterRecursive();
		StatementSyntax result;

		if (Current.TKind == TokenKind.OpenBrace)
		{
			result = ParseBlockStatement();
		}
		else if (Current.TKind == TokenKind.Break)
		{
			result = ParseBreakStatement();
		}
		else if (Current.TKind == TokenKind.Return)
		{
			result = ParseReturnStatement();
		}
		else if (Current.TKind == TokenKind.Let)
		{
			result = ParseLocalVariableDeclarationStatement();
		}
		else if (Current.TKind == TokenKind.If)
		{
			result = ParseIfStatement();
		}
		else if (Current.TKind == TokenKind.Loop)
		{
			result = ParseLoopStatement();
		}
		else if (Current.TKind == TokenKind.While)
		{
			result = ParseWhileStatement();
		}
		else if (Current.TKind.IsAnyIdentifierOrKeyword ||
		         Current.TKind.IsLiteralTokenKind ||
		         Current.TKind == TokenKind.Self ||
		         Current.TKind == TokenKind.OpenParen ||
		         Current.TKind == TokenKind.Ampersand ||
		         Current.TKind == TokenKind.Asterisk ||
		         (Current.TKind is { IsOperator: true, CanBeUnaryOperator: true }))
		{
			result = ParseExpressionStatement();
		}
		else
		{
			TokenKind currentKind = Current.TKind;

			SyntaxList<Token>.Builder erroredNodes = new();
			while (currentKind != TokenKind.EndOfFile &&
			       currentKind != TokenKind.Semicolon &&
			       currentKind != TokenKind.OpenBrace &&
			       currentKind != TokenKind.CloseBrace)
			{
				erroredNodes.Add(PeekAndAdvance());
				currentKind = Current.TKind;
			}

			var nodes = erroredNodes.Build(_syntaxTree);

			if (nodes.Count == 0)
			{
				Token token = PeekAndAdvance();
				SyntaxList<Token>.Builder singleTokenBuilder = new();
				singleTokenBuilder.Add(token);
				nodes = singleTokenBuilder.Build(_syntaxTree);
			}

			_diagnostics.ReportUnexpectedToken(nodes[0].Location, nodes[0].TKind);
			result = new ErrorStatementSyntax(_syntaxTree, nodes);
		}

		LeaveRecursive();
		return result;
	}

	private BlockStatementSyntax ParseBlockStatement()
	{
		EnterRecursive();
		Token openBrace = MatchToken(TokenKind.OpenBrace);

		SyntaxList<StatementSyntax>.Builder statementsBuilder = new();
		while (Current.TKind != TokenKind.CloseBrace)
		{
			if (Current.TKind == TokenKind.EndOfFile)
			{
				_diagnostics.ReportExpectedToken(
					new TextSpan(int.Max(Current.Span.Start - 1, 0), 1).Contextualize(_syntaxTree),
					TokenKind.CloseBrace
				);

				LeaveRecursive();
				return new(openBrace.Tree, openBrace, statementsBuilder.Build(openBrace.Tree), MatchToken(TokenKind.CloseBrace));
			}

			StatementSyntax node = ParseStatement();
			statementsBuilder.Add(node);
		}
		Token closeBrace = MatchToken(TokenKind.CloseBrace);

		LeaveRecursive();
		return new(openBrace.Tree, openBrace, statementsBuilder.Build(_syntaxTree), closeBrace);
	}

	private ExpressionStatementSyntax ParseExpressionStatement()
	{
		ExpressionSyntax expr = ParseExpression();
		Token semicolon = MatchToken(TokenKind.Semicolon);
		return new(expr.Tree, expr, semicolon);
	}

	private ReturnStatementSyntax ParseReturnStatement()
	{
		Token returnKeyword = MatchToken(TokenKind.Return);

		if (Current.TKind == TokenKind.Semicolon)
		{
			return new(returnKeyword.Tree, returnKeyword, null, PeekAndAdvance());
		}
		else
		{
			ExpressionSyntax expression = ParseExpression();
			Token semicolon = MatchToken(TokenKind.Semicolon);
			return new(returnKeyword.Tree, returnKeyword, expression, semicolon);
		}
	}

	private LocalVariableDeclarationStatement ParseLocalVariableDeclarationStatement()
	{
		Token let = MatchToken(TokenKind.Let);

		LocalVariableDeclarator declarator = ParseLocalVariableDeclarator();

		Token semicolon = MatchToken(TokenKind.Semicolon);

		return new(_syntaxTree, let, declarator, semicolon);
	}

	private LocalVariableDeclarator ParseLocalVariableDeclarator()
	{
		SimpleNameSyntax name = ParseSimpleName();

		TypeClauseSyntax? typeClause = null;
		EqualsValueClause? valueClause = null;

		if (Current.TKind == TokenKind.Colon)
		{
			Token colon = PeekAndAdvance();
			TypeSyntax type = ParseType();
			typeClause = new(_syntaxTree, colon, type);
		}

		if (Current.TKind == TokenKind.Equal)
		{
			Token equalsToken = PeekAndAdvance();
			ExpressionSyntax expression = ParseExpression();
			valueClause = new(_syntaxTree, equalsToken, expression);
		}

		return new(_syntaxTree, name, typeClause, valueClause);
	}

	private IfStatementSyntax ParseIfStatement()
	{
		Token ifToken = MatchToken(TokenKind.If);

		ExpressionSyntax conditionExpression = ParseExpression();

		StatementSyntax thenStatement = ParseStatement();
		ElseClauseSyntax? elseClause = null;
		if (Current.TKind == TokenKind.Else)
		{
			elseClause = ParseElseClause();
		}

		return new IfStatementSyntax(_syntaxTree, ifToken, conditionExpression, thenStatement, elseClause);
	}

	private ElseClauseSyntax ParseElseClause()
	{
		Token elseToken = MatchToken(TokenKind.Else);

		StatementSyntax elseStatement = ParseStatement();

		return new ElseClauseSyntax(_syntaxTree, elseToken, elseStatement);
	}

	private LoopStatementSyntax ParseLoopStatement()
	{
		Token loopToken = MatchToken(TokenKind.Loop);

		StatementSyntax body = ParseStatement();

		return new LoopStatementSyntax(_syntaxTree, loopToken, body);
	}

	private WhileStatementSyntax ParseWhileStatement()
	{
		Token @while = MatchToken(TokenKind.While);

		ExpressionSyntax condition = ParseExpression();
		StatementSyntax body = ParseStatement();

		return new WhileStatementSyntax(_syntaxTree, @while, condition, body);
	}

	private BreakStatementSyntax ParseBreakStatement()
	{
		Token breakKeyword = MatchToken(TokenKind.Break);
		Token semicolon = MatchToken(TokenKind.Semicolon);
		return new BreakStatementSyntax(_syntaxTree, breakKeyword, semicolon);
	}

	// private ForStatementSyntax ParseForStatement()
	// {
	// 	Token @for = MatchToken(TokenKind.For);
	//
	// 	throw new NotImplementedException();
	// }
	//
	// private DoWhileStatementSyntax ParseDoWhileStatement()
	// {
	// 	Token @do = MatchToken(TokenKind.Do);
	//
	// 	throw new NotImplementedException();
	// }
}