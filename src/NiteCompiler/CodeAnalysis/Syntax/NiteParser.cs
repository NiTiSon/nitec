using System;
using System.Collections.Generic;
using System.Diagnostics;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed partial class NiteParser
{
	private readonly DiagnosticBag _diagnostics;
	private readonly List<Token> _tokens;
	private int _position;
	private readonly SyntaxTree _syntaxTree;

	public NiteParser(NiteLexer lexer, SyntaxTree syntaxTree, DiagnosticBag diagnostics)
	{
		_diagnostics = diagnostics;
		_tokens = new(capacity: 64);
		_syntaxTree = syntaxTree;

		Token token;
		do
		{
			token = lexer.Lex();
			_tokens.Add(token);
		} while (token.TKind != TokenKind.EndOfFile);
	}

	public Token Current => Peek(0);

	[DebuggerStepThrough]
	private Token Peek(int offset)
	{
		return _tokens[int.Min(offset + _position, _tokens.Count - 1)];
	}

	[DebuggerStepThrough]
	private Token PeekAndAdvance()
	{
		Token current = Current;
		_position++;
		return current;
	}

	[DebuggerStepThrough]
	private void Advance()
	{
		_position++;
	}

	[DebuggerStepThrough]
	private Token MatchToken(TokenKind kind)
	{
		if (Current.TKind == kind)
			return PeekAndAdvance();

		_diagnostics.ReportExpectedToken(Current.Span.Contextualize(_syntaxTree), kind);
		return new Token.Default(Current.Tree, kind, Current.Span, Current.LeadingTrivia, Current.TrailingTrivia);
	}

	private bool IsPresentedAny(params ReadOnlySpan<TokenKind> kinds)
	{
		return kinds.Contains(Current.TKind);
	}

	private bool IsPresentedAnyAccessibilityToken()
	{
		return IsPresentedAny(TokenKind.Public, TokenKind.Friend, TokenKind.Protected, TokenKind.Internal, TokenKind.Family, TokenKind.Private);
	}

	private bool IsPresentedContextualKeyword(TokenKind contextualKeywordType)
	{
		Debug.Assert(contextualKeywordType.IsKeyword);
		return Current.TKind.GetContextualKeyword() == contextualKeywordType;
	}

	[DebuggerStepThrough]
	private Token MatchAnyToken(params ReadOnlySpan<TokenKind> kinds)
	{
		if (IsPresentedAny(kinds))
			return PeekAndAdvance();

		return new Token.Default(Current.Tree, TokenKind.None, Current.Span, Current.LeadingTrivia, Current.TrailingTrivia);
	}

	public CompilationUnitSyntax Parse()
	{
		SyntaxList<TopLevelSyntax>.Builder itemsBuilder = new();
		while (Current.TKind != TokenKind.EndOfFile)
		{
			switch (Current.TKind)
			{
				// case SyntaxKind.UseKeyword:
				// 	membersBuilder.Add(ParseUseDirective());
				// 	MatchToken(SyntaxKind.SemicolonToken);
				// 	break;
				// case SyntaxKind.ModuleKeyword:
				// 	membersBuilder.Add(ParseModuleDeclaration());
				// 	MatchToken(SyntaxKind.SemicolonToken);
				// 	break;
				default:
					if (IsPresentedAnyAccessibilityToken())
					{
						itemsBuilder.Add(ParseItem(Current));
					}
					else
					{
						_diagnostics.ReportUnexpectedToken(Current.Span.Contextualize(_syntaxTree), Current.TKind);
						_position++;
					}
					break;
			}
		}
		Token endOfFileToken = MatchToken(TokenKind.EndOfFile);

		return new CompilationUnitSyntax(_syntaxTree, itemsBuilder.Build(_syntaxTree), endOfFileToken);
	}
}