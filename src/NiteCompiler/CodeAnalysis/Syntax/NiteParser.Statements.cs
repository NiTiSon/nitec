using System;
using System.Collections.Immutable;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed partial class NiteParser
{
	private BlockStatementSyntax ParseBlockStatement()
	{
		Token openBrace = MatchToken(SyntaxKind.OpenBraceToken);

		ImmutableArray<StatementSyntax>.Builder statements = ImmutableArray.CreateBuilder<StatementSyntax>();
		while (Current.Kind != SyntaxKind.CloseBraceToken)
		{
			if (Current.Kind == SyntaxKind.EndOfFile)
			{
				// report
				return new BlockStatementSyntax(openBrace, [], Current);
			}

			statements.Add(ParseStatement());
			if (Current.Kind == SyntaxKind.SemicolonToken) // Change to MatchToken to make semicolon explicit
			{
				Advance();
			}
		}

		Token closeBrace = MatchToken(SyntaxKind.CloseBraceToken);

		return new BlockStatementSyntax(openBrace, statements.ToImmutable(), closeBrace);
	}

	private StatementSyntax ParseStatement()
	{
		if (Current.Kind == SyntaxKind.OpenBraceToken)
		{
			return ParseBlockStatement();
		}
		else if (Current.Kind == SyntaxKind.ReturnKeyword)
		{
			return ParseReturnStatement();
		}
		else
		{
			return ParseExpressionStatement();
		}
	}

	private ExpressionStatementSyntax ParseExpressionStatement()
	{
		return new(ParseExpression());
	}

	private ReturnStatementSyntax ParseReturnStatement()
	{
		Token returnKeyword = MatchToken(SyntaxKind.ReturnKeyword);
		return Current.Kind == SyntaxKind.SemicolonToken
			? new(returnKeyword, null)
			: new(returnKeyword, ParseExpression());
	}
}