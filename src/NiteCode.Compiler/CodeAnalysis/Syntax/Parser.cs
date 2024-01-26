using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

internal sealed class Parser
{
	private readonly SyntaxTree tree;
	private readonly ImmutableArray<SyntaxToken> tokens;
	private readonly DiagnosticBag diagnostics;

	public DiagnosticBag Diagnostics
		=> diagnostics;

	private uint pos;

	private SyntaxToken Current => Peek(0);
	private SyntaxToken Next => Peek(1);

	private SyntaxToken Peek(int offset)
	{
		int index = (int)pos + offset;
		if (index >= tokens.Length)
			return tokens[^1];

		return tokens[index];
	}

	private SyntaxToken NextToken()
	{
		SyntaxToken current = Current;
		pos++;
		return current;
	}

	private SyntaxToken MatchToken(SyntaxKind kind)
	{
		if (Current.Kind == kind)
			return NextToken();

		diagnostics.ReportUnexpectedToken(Current.Location, Current.Kind, kind);
		return new SyntaxToken(tree, kind, Current.Position, StringSegment.Empty, ImmutableArray<SyntaxTrivia>.Empty, ImmutableArray<SyntaxTrivia>.Empty);
	}

	public Parser(SyntaxTree tree)
	{
		this.tree = tree;
		List<SyntaxToken> tokens = [];
		List<SyntaxToken> badTokens = [];
		diagnostics = [];

		Lexer lexer = new(tree);
		SyntaxToken token;
		do
		{
			token = lexer.NextToken();

			if (token.Kind == SyntaxKind.BadToken)
			{
				badTokens.Add(token);
			}
			else
			{
				if (badTokens.Count > 0)
				{
					ImmutableArray<SyntaxTrivia>.Builder leadingTrivia = token.LeadingTrivia.ToBuilder();
					int index = 0;

					foreach (SyntaxToken badToken in badTokens)
					{
						foreach (SyntaxTrivia lt in badToken.LeadingTrivia)
							leadingTrivia.Insert(index++, lt);

						SyntaxTrivia trivia = new(tree, SyntaxKind.SkippedTokensTrivia, badToken.Position, badToken.Text);
						leadingTrivia.Insert(index++, trivia);

						foreach (SyntaxTrivia tt in badToken.TrailingTrivia)
							leadingTrivia.Insert(index++, tt);
					}

					badTokens.Clear();
					token = new SyntaxToken(token.SyntaxTree, token.Kind, token.Position, token.Text, leadingTrivia.ToImmutable(), token.TrailingTrivia);
				}

				tokens.Add(token);
			}
		} while (token.Kind != SyntaxKind.EndOfFileToken);

		this.tree = tree;
		this.tokens = tokens.ToImmutableArray();
		diagnostics.AddRange(lexer.Diagnostics);
	}

	public CompilationUnitSyntax ParseCompilationUnit()
	{
		throw new NotImplementYetException();
	}

	public ExpressionSyntax ParseExpression(uint parentPrecedence = 0)
	{
		ExpressionSyntax left;
		uint unaryOperatorPrecedence = Current.GetUnaryOperatorPrecedence();

		if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecedence)
		{
			SyntaxToken operatorToken = NextToken();
			ExpressionSyntax operand = ParseExpression(unaryOperatorPrecedence);
			left = new UnaryExpressionSyntax(tree, TokenKindToUnaryExpressionKind(operatorToken.Kind), operatorToken, operand);
		}
		else
		{
			left = ParsePrimaryExpression();
		}

		while (true)
		{
			uint precedence = Current.Kind.GetBinaryOperatorPrecedence();
			if (precedence == 0 || precedence <= parentPrecedence)
				break;

			SyntaxToken operatorToken = NextToken();
			ExpressionSyntax right = ParseExpression(precedence);
			left = new BinaryExpressionSyntax(tree, TokenKindToBinaryExpressionKind(operatorToken.Kind), left, operatorToken, right);
		}

		return left;
	}

	private ExpressionSyntax ParsePrimaryExpression()
	{
		switch (Current.Kind)
		{
			case SyntaxKind.OpenParenToken:
				{
					SyntaxToken left = NextToken();
					ExpressionSyntax expression = ParseExpression();
					SyntaxToken right = MatchToken(SyntaxKind.CloseParenToken);
					return new ParenthesizedExpressionSyntax(tree, left, expression, right);
				}

			//case SyntaxKind.FalseKeyword:
			//case SyntaxKind.TrueKeyword:
			//	{
			//		var keywordToken = NextToken();
			//		var value = keywordToken.Kind == SyntaxKind.TrueKeyword;
			//		return new LiteralExpressionSyntax(keywordToken, value);
			//	}

			default:
				{
					SyntaxToken numberToken = MatchToken(SyntaxKind.NumericLiteralToken);
					return new NumericLiteralExpression(tree, numberToken);
				}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static SyntaxKind TokenKindToBinaryExpressionKind(SyntaxKind kind)
	{
		return kind switch
		{
			SyntaxKind.PlusToken => SyntaxKind.AddExpression,
			SyntaxKind.MinusToken => SyntaxKind.SubstractExpression,
			SyntaxKind.SlashToken => SyntaxKind.DivideExpression,
			SyntaxKind.AsteriskToken => SyntaxKind.MultiplyExpression,
			SyntaxKind.PercentToken => SyntaxKind.ModuloExpression,
			_ => throw new NotImplementYetException()
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static SyntaxKind TokenKindToUnaryExpressionKind(SyntaxKind kind)
	{
		return kind switch
		{
			SyntaxKind.TildeToken => SyntaxKind.BitwiseNotExpression,
			_ => throw new NotImplementYetException()
		};
	}
}
