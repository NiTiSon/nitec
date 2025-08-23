using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Syntax.Directives;
using NiteCompiler.CodeAnalysis.Syntax.Expressions.Names;
using NiTiS.Compiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class NiteParser
{
	private readonly DiagnosticBag _diagnostics;
	private readonly List<Token> _tokens;
	private int _position;

	public NiteParser(NiteLexer lexer, DiagnosticBag diagnostics)
	{
		_diagnostics = diagnostics;
		Token token = lexer.Lex();
		_tokens = new(capacity: 64);

		do
		{
			_tokens.Add(token);
			token = lexer.Lex();
		} while (token.Kind != SyntaxKind.EndOfFile);
	}

	public Token Current => Peek(0);

	private Token Peek(int offset)
	{
		return _tokens[int.Min(offset + _position, _tokens.Count)];
	}
	private Token PeekAndAdvance()
	{
		Token current = Current;
		_position++;
		return current;
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

	public SyntaxTree Parse()
	{
		var usagesBuilder = ImmutableArray.CreateBuilder<UseDirectiveSyntax>();
		while (Current.Kind != SyntaxKind.EndOfFile)
		{
			switch (Current.Kind)
			{
				case SyntaxKind.UseKeyword:
					usagesBuilder.Add(ParseUseDirective());
					break;
				default:
					_position++;
					break;
			}
		}

		return new SyntaxTree(usagesBuilder.DrainToImmutable());
	}

	private UseDirectiveSyntax ParseUseDirective()
	{
		Token useKeyword = MatchToken(SyntaxKind.UseKeyword);
		ModuleNameSyntax moduleName = ParseModuleName();
	}

	private ModuleNameSyntax ParseModuleName()
	{
		if (Current.Kind == SyntaxKind.IdentifierToken)
		{

		}
	}
}