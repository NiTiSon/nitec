using System;
using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed partial class NiteParser
{
	private BlockStatementSyntax ParseBlockStatement()
	{
		Token openBrace = MatchToken(SyntaxKind.OpenBraceToken);

		var statementsBuilder = ImmutableArray.CreateBuilder<StatementSyntax>();
		while (Current.Kind != SyntaxKind.CloseBraceToken)
		{
			if (Current.Kind == SyntaxKind.EofToken)
			{
				_diagnostics.ReportExpectedToken(
					new TextSpan(int.Max(Current.Span.Start - 1, 0), 1).Contextualize(_syntaxTree),
					SyntaxKind.CloseBraceToken);
				return new BlockStatementSyntax(openBrace, [], Current);
			}

			StatementSyntax node = ParseStatement();
			statementsBuilder.Add(node);
			if (!node.IsRequiresSemicolon) continue;

			if (Current.Kind == SyntaxKind.SemicolonToken)
			{
				Advance();
			}
			else
			{
				_diagnostics.ReportExpectedToken(Current.Span.Contextualize(_syntaxTree), SyntaxKind.SemicolonToken);
			}
		}

		Token closeBrace = MatchToken(SyntaxKind.CloseBraceToken);

		return new BlockStatementSyntax(openBrace, statementsBuilder.ToImmutable(), closeBrace);
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