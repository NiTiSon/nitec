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
		_tokens = new(capacity: 64);

		Token token;
		do
		{
			token = lexer.Lex();
			_tokens.Add(token);
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

	public SyntaxTree Parse()
	{
		ImmutableArray<ISyntaxTreeTopLevelMember>.Builder membersBuilder = ImmutableArray.CreateBuilder<ISyntaxTreeTopLevelMember>();
		while (Current.Kind != SyntaxKind.EndOfFile)
		{
			switch (Current.Kind)
			{
				case SyntaxKind.UseKeyword:
					membersBuilder.Add(ParseUseDirective());
					break;
				default:
					_position++;
					break;
			}
		}

		return new SyntaxTree(membersBuilder.DrainToImmutable());
	}

	private UseDirectiveSyntax ParseUseDirective()
	{
		Token useKeyword = MatchToken(SyntaxKind.UseKeyword);
		ModuleNameSyntax moduleName = ParseModuleName();

		return new(useKeyword, moduleName);
	}

	private ModuleNameSyntax ParseModuleName()
	{
		SyntaxList<IdentifierNameSyntax>.Builder parts = new(SyntaxKind.IdentifierList);

		parts.Add(ParseIdentifierName());

		while (Current.Kind == SyntaxKind.ColonColonToken)
		{
			Advance();
			if (Current.Kind == SyntaxKind.IdentifierToken)
			{
				parts.Add(ParseIdentifierName());
			}
			else
			{
				// _diagnostics.Report
				break;
			}
		}

		return new ModuleNameSyntax(parts.Build());
	}

	private IdentifierNameSyntax ParseIdentifierName()
	{
		return new((MatchToken(SyntaxKind.IdentifierToken) as IdentifierToken)!);
	}
}