using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Syntax.Expressions.Names;
using NiteCompiler.CodeAnalysis.Text;
using NiTiS.Compiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class NiteParser
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

	private Token MatchAnyToken(params ReadOnlySpan<SyntaxKind> kinds)
	{
		if (IsPresentedAny(kinds))
			return PeekAndAdvance();

		return new Token(SyntaxKind.Invalid, Current.Span);
	}

	public CompilationUnitSyntax Parse()
	{
		ImmutableArray<SyntaxNode>.Builder membersBuilder = ImmutableArray.CreateBuilder<SyntaxNode>();
		while (Current.Kind != SyntaxKind.EndOfFile)
		{
			switch (Current.Kind)
			{
				case SyntaxKind.UseKeyword:
					membersBuilder.Add(ParseUseDirective());
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
		Token endOfFileToken = MatchToken(SyntaxKind.EndOfFile);

		return new CompilationUnitSyntax(_source, membersBuilder.ToImmutable(), endOfFileToken);
	}

	private SyntaxNode ParseMember()
	{
		MatchAnyToken(SyntaxFacts.AccessKeywords);

		IdentifierNameSyntax name = ParseIdentifierName();

		if (Current.Kind == SyntaxKind.OpenParenToken) // Method
		{
			Token openParen = MatchToken(SyntaxKind.OpenParenToken);
			Token closeParen = MatchToken(SyntaxKind.CloseParenToken);

			if (Current.Kind == SyntaxKind.RetusaToken)
			{
				Token retusa = MatchToken(SyntaxKind.RetusaToken);

				NameSyntax returnParameter = ParseName();
			}

			BlockStatementSyntax block = ParseBlockStatement();
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
		}
		Token closeBrace = MatchToken(SyntaxKind.CloseBraceToken);

		return new BlockStatementSyntax(openBrace, statements.ToImmutable(), Current);
	}

	private StatementSyntax ParseStatement()
	{
		if (Current.Kind == SyntaxKind.OpenBraceToken)
		{
			return ParseBlockStatement();
		}
		else
		{
			throw new NotImplementedException();
		}
	}

	private NameSyntax ParseName()
	{
		Token next = Peek(1);
		ModuleNameSyntax? moduleName = null;
		if (next.Kind == SyntaxKind.ColonColonToken) // Qualified name
		{
			 moduleName = ParseModuleName();
		}

		if (Current.Kind == SyntaxKind.ColonColonToken)
		{
			Token colonColonToken = PeekAndAdvance();

			NameSyntax nameSyntax = ParseIdentifierName(); // Replace with simple name for generics support.

			return new QualifiedNameSyntax(moduleName!, colonColonToken, nameSyntax);
		}
		else
		{
			return ParseIdentifierName();
		}
	}

	private UseDirectiveSyntax ParseUseDirective()
	{
		Token useKeyword = MatchToken(SyntaxKind.UseKeyword);
		ModuleNameSyntax moduleName = ParseModuleNameInUseDirective();

		return new(useKeyword, moduleName);
	}

	private ModuleNameSyntax ParseModuleName()
	{
		SyntaxList<IdentifierNameSyntax>.Builder parts = new(SyntaxKind.IdentifierList);

		parts.Add(ParseIdentifierName());

		while (Current.Kind == SyntaxKind.ColonColonToken)
		{
			Token next = Peek(1);
			if (next.Kind == SyntaxKind.IdentifierToken && Peek(2).Kind == SyntaxKind.ColonColonToken)
			{
				Advance(); // ::
				parts.Add(ParseIdentifierName());
			}
			else
			{
				// The rest is ModuleMemberAccessExpression
				break;
			}
		}

		return new ModuleNameSyntax(parts.Build());
	}

	/// <summary>
	/// In use directives only module names appears without any members. This method reads whole path as module name.
	/// </summary>
	private ModuleNameSyntax ParseModuleNameInUseDirective()
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