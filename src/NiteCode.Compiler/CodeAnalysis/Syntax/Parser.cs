using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

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

	public Parser(SyntaxTree tree)
	{
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
					var leadingTrivia = token.LeadingTrivia.ToBuilder();
					var index = 0;

					foreach (var badToken in badTokens)
					{
						foreach (var lt in badToken.LeadingTrivia)
							leadingTrivia.Insert(index++, lt);

						var trivia = new SyntaxTrivia(tree, SyntaxKind.SkippedTextTrivia, badToken.Position, badToken.Text);
						leadingTrivia.Insert(index++, trivia);

						foreach (var tt in badToken.TrailingTrivia)
							leadingTrivia.Insert(index++, tt);
					}

					badTokens.Clear();
					token = new SyntaxToken(token.SyntaxTree, token.Kind, token.Position, token.Text, leadingTrivia.ToImmutable(), token.TrailingTrivia);
				}

				tokens.Add(token);
			}
		} while (token.Kind != SyntaxKind.EndOfFile);

		this.tree = tree;
		this.tokens = tokens.ToImmutableArray();
		diagnostics.AddRange(lexer.Diagnostics);
	}

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


	public CompilationUnitSyntax ParseCompilationUnit()
	{
		ImmutableArray<UsingSyntax>.Builder usings = ImmutableArray.CreateBuilder<UsingSyntax>();
		SyntaxToken eof;

		while (Current.Kind == SyntaxKind.UsingKeyword)
		{
			UsingSyntax @using = ParseUsingSyntax();

			usings.Add(@using);
		}
		while (Current.Kind != SyntaxKind.EndOfFile)
		{
			NextToken();
		}
		eof = Current;

		return new CompilationUnitSyntax(tree, [], usings.ToImmutable(), eof);
	}

	private UsingSyntax ParseUsingSyntax()
	{
		SyntaxToken usingKeyword = MatchToken(SyntaxKind.UsingKeyword);
		NameSyntax module = ParseNameSyntax();
		SyntaxToken semicolon = MatchToken(SyntaxKind.Semicolon);

		return new UsingSyntax(tree, usingKeyword, module, semicolon);
	}

	private NameSyntax ParseNameSyntax()
	{
		IdentifierNameSyntax firstName = ParseIdentifierName();

		if (Current.Kind == SyntaxKind.Dot)
		{
			SyntaxToken dot = NextToken();

			NameSyntax rightName = ParseNameSyntax();

			return new QualifiedNameSyntax(tree, firstName, dot, rightName);
		}
		else
		{
			return firstName;
		}
	}

	private IdentifierNameSyntax ParseIdentifierName()
	{
		return new IdentifierNameSyntax(tree, MatchToken(SyntaxKind.Identifier));
	}

	public BinaryExpressionSyntax ParseBinaryExpression()
	{
		ExpressionSyntax left = ParsePrimaryExpression();
		SyntaxToken @operator;
		if (Current.Kind
			is SyntaxKind.Plus
			or SyntaxKind.Minus
			or SyntaxKind.Slash
			or SyntaxKind.Asterisk
			or SyntaxKind.Modulo
			)
		{
			@operator = Current;
			pos++;
		}
		else
		{
			throw new NotImplementedException();
		}
		ExpressionSyntax right = ParsePrimaryExpression();

		return new(tree, left, @operator, right);
	}
	public ExpressionSyntax ParsePrimaryExpression()
	{
		switch (Current.Kind)
		{
			case SyntaxKind.OpenParenthesis:
				throw new NotImplementedException();
				//return ParseParenthesizedExpression();

			case SyntaxKind.FalseConst:
			case SyntaxKind.TrueConst:
				throw new NotImplementedException();
				//return ParseBooleanLiteral();

			case SyntaxKind.Number:
				return ParseNumberLiteral();

			case SyntaxKind.String:
				throw new NotImplementedException();
				//return ParseStringLiteral();

			case SyntaxKind.Identifier:
				throw new NotImplementedException();
				//return ParseNameOrCallExpression();
			default:
				throw new NotImplementedException();
		}
	}
	private ExpressionSyntax ParseNumberLiteral()
	{
		SyntaxToken numberToken = MatchToken(SyntaxKind.Number);
		return new LiteralExpressionSyntax(tree, numberToken);
	}
}