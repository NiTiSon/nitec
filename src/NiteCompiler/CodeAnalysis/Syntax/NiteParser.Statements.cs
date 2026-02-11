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

			if (Current.TKind == TokenKind.Semicolon)
			{
				Token semicolon = MatchToken(TokenKind.Semicolon);
				statementsBuilder.Add(new EmptyStatementSyntax(semicolon.Tree, semicolon));
			}
			else
			{
				_diagnostics.ReportExpectedToken(Current.Span.Contextualize(_syntaxTree), TokenKind.Semicolon);
			}
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
		return Current.TKind == TokenKind.Semicolon
			? new(returnKeyword.Tree, returnKeyword, null, MatchToken(TokenKind.Semicolon))
			: new(returnKeyword.Tree, returnKeyword, ParseExpression(), MatchToken(TokenKind.Semicolon));
	}
}