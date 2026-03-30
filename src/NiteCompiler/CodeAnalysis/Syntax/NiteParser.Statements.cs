using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed partial class NiteParser
{
	private StatementSyntax ParseStatement()
	{
		if (Current.TKind == TokenKind.OpenBrace)
		{
			return ParseBlockStatement();
		}
		else if (Current.TKind == TokenKind.Return)
		{
			return ParseReturnStatement();
		}
		else if (Current.TKind == TokenKind.Let)
		{
			return ParseLocalVariableDeclarationStatement();
		}
		else if (Current.TKind == TokenKind.If)
		{
			return ParseIfStatement();
		}
		else
		{
			return ParseExpressionStatement();
		}
	}

	private BlockStatementSyntax ParseBlockStatement()
	{
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

				return new(openBrace.Tree, openBrace, statementsBuilder.Build(openBrace.Tree), Current);
			}

			StatementSyntax node = ParseStatement();
			statementsBuilder.Add(node);
		}
		Token closeBrace = MatchToken(TokenKind.CloseBrace);

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

		TypeClause? typeClause = null;
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
}