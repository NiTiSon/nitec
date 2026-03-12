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
		if (Current.TKind != TokenKind.IdentifierOrKeyword) return false;

		ReadOnlySpan<ushort> s = [
			TokenKind.Public.RawValue,
			TokenKind.Friend.RawValue,
			TokenKind.Protected.RawValue,
			TokenKind.Internal.RawValue,
			TokenKind.Family.RawValue,
			TokenKind.Private.RawValue];

		return s.Contains(Current.TKind.GetContextualKeyword().RawValue);
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
		SyntaxList<ItemSyntax>.Builder itemsBuilder = new();
		while (Current.TKind != TokenKind.EndOfFile)
		{
			if (IsPresentedAnyAccessibilityToken())
			{
				// All accessibility tokens are contextual
				Token elevatedKeyword = PeekAndAdvance().ToContextualKeywordToken();
				itemsBuilder.Add(ParseMember(elevatedKeyword));
			}
			else if (Current.TKind == TokenKind.Module)
			{
				Token moduleKeyword = PeekAndAdvance();
				itemsBuilder.Add(ParseModuleDeclaration(moduleKeyword));
			}
			else
			{
				_diagnostics.ReportUnexpectedToken(Current.Span.Contextualize(_syntaxTree), Current.TKind);
				_position++;
			}
		}
		Token endOfFileToken = MatchToken(TokenKind.EndOfFile);

		return new CompilationUnitSyntax(_syntaxTree, itemsBuilder.Build(_syntaxTree), endOfFileToken);
	}

	private ItemSyntax ParseModuleDeclaration(Token moduleKeyword)
	{
		ModuleNameSyntax name = ParseModuleName();
		Token semicolon = MatchToken(TokenKind.Semicolon);

		SyntaxList<MemberSyntax>.Builder membersBuilder = new();

		while (IsPresentedAnyAccessibilityToken())
		{
			Token elevatedKeyword = PeekAndAdvance().ToContextualKeywordToken();
			membersBuilder.Add(ParseMember(elevatedKeyword));
		}

		return new ModuleDeclarationSyntax(_syntaxTree, moduleKeyword, name, semicolon, membersBuilder.Build(_syntaxTree));
	}
}