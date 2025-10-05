using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using CommunityToolkit.Diagnostics;
using NiteCompiler.CodeAnalysis.Text;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed partial class NiteParser
{
	private readonly DiagnosticBag _diagnostics;
	private readonly List<Token> _tokens;
	private int _position;
	private SourceText _source;

	public NiteParser(NiteLexer lexer, StringText sourceText, DiagnosticBag diagnostics)
	{
		_diagnostics = diagnostics;
		_tokens = new(capacity: 64);
		_source = sourceText;

		Token token;
		do
		{
			token = lexer.Lex();
			_tokens.Add(token);
		} while (token.Kind != SyntaxKind.EofToken); // Also include EOF token

		#if DEBUG
		Console.WriteLine("Token/Character ratio: " + (float)_tokens.Count / sourceText.Length);
		#endif
	}

	public Token Current => Peek(0);

	private Token Peek(int offset)
	{
		return _tokens[int.Min(offset + _position, _tokens.Count - 1)];
	}
	private Token PeekAndAdvance()
	{
		Token current = Current;
		_position++;
		return current;
	}

	private void Advance()
	{
		_position++;
	}

	private Token MatchToken(SyntaxKind kind)
	{
		if (Current.Kind == kind)
			return PeekAndAdvance();

		return new Token(kind, Current.Span);
	}

	private bool IsPresentedAny(params ReadOnlySpan<SyntaxKind> kinds)
	{
		return kinds.Contains(Current.Kind);
	}

	private Token MatchAnyToken(params ReadOnlySpan<SyntaxKind> kinds)
	{
		if (IsPresentedAny(kinds))
			return PeekAndAdvance();

		return new Token(SyntaxKind.None, Current.Span);
	}

	public CompilationUnitSyntax Parse()
	{
		ImmutableArray<SyntaxNode>.Builder membersBuilder = ImmutableArray.CreateBuilder<SyntaxNode>();
		while (Current.Kind != SyntaxKind.EofToken)
		{
			switch (Current.Kind)
			{
				case SyntaxKind.UseKeyword:
					membersBuilder.Add(ParseUseDirective());
					break;
				case SyntaxKind.ModuleKeyword:
					membersBuilder.Add(ParseModuleDeclaration());
					break;
				default:
					if (IsPresentedAny(SyntaxFacts.AccessKeywords))
					{
						membersBuilder.Add(ParseMember());
					}
					_position++;
					break;
			}
		}
		Token endOfFileToken = MatchToken(SyntaxKind.EofToken);

		return new CompilationUnitSyntax(_source, membersBuilder.ToImmutable(), endOfFileToken);
	}

	private SyntaxNode ParseMember()
	{
		Token accessibilityToken = MatchAnyToken(SyntaxFacts.AccessKeywords);

		SimpleNameSyntax name = ParseSimpleName();

		if (Current.Kind == SyntaxKind.OpenParenToken) // Method
		{
			Token openParen = MatchToken(SyntaxKind.OpenParenToken);
			Token closeParen = MatchToken(SyntaxKind.CloseParenToken);

			RetusaClauseSyntax? retusa = null;
			if (Current.Kind == SyntaxKind.RetusaToken)
			{
				Token retusaArrow = MatchToken(SyntaxKind.RetusaToken);

				TypeSyntax returnParameter = ParseType();
				retusa = new(retusaArrow, returnParameter);
			}

			BlockStatementSyntax block = ParseBlockStatement();

			return new FunctionDeclarationSyntax(accessibilityToken, [], name, null, retusa, block);
		}
		// else // field
		// {
		// 	if (Current.Kind == SyntaxKind.ColonToken) // field: type
		// 	{
		// 		NameSyntax typeName = ParseTypeName();
		// 	}
		// }
		return name;
	}

	private (SyntaxKind operatorTokenKind, SyntaxKind operatorExpressionKind) GetExpressionOperatorTokenKindAndExpressionKind()
	{
		if (SyntaxFacts.IsBinaryExpressionOperatorToken(Current.Kind))
			return (Current.Kind, SyntaxFacts.GetBinaryExpression(Current.Kind));

		if (SyntaxFacts.IsAssignmentExpressionOperatorToken(Current.Kind))
			return (Current.Kind, SyntaxFacts.GetAssignmentExpression(Current.Kind));

		return (SyntaxKind.None, SyntaxKind.None);
	}

	private ModuleDeclarationSyntax ParseModuleDeclaration()
	{
		Token moduleKeyword = MatchToken(SyntaxKind.ModuleKeyword);
		ModuleNameSyntax moduleName = ParseModuleNameAlone();

		return new(moduleKeyword, moduleName);
	}

	private UseDirectiveSyntax ParseUseDirective()
	{
		Token useKeyword = MatchToken(SyntaxKind.UseKeyword);
		ModuleNameSyntax moduleName = ParseModuleNameAlone();

		return new(useKeyword, moduleName);
	}
}