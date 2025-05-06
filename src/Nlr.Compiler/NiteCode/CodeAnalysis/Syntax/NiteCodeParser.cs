using System;
using System.Collections.Immutable;
using Nlr.Compiler.CodeAnalysis.Syntax;
using Nlr.Compiler.Extensions;
using static Nlr.Compiler.CodeAnalysis.Syntax.SyntaxKind;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public sealed class NiteCodeParser
{
	private readonly ImmutableArray<Token> _rawTokens;

	private int _position;

	private ModuleSyntax? _currentModule;
	
	public NiteCodeParser(NiteCodeLexer lexer)
	{
		ImmutableArray<Token>.Builder tokens = ImmutableArray.CreateBuilder<Token>();
		Token token;
		do
		{
			// TODO: Remove bad tokens from parsing: any UnknownOrWrong token should produce diagnostic error
			token = lexer.Lex();
			tokens.Add(token);
		}
		while (token.Kind != EndOfFile);
		
		_rawTokens = tokens.ToImmutable();

		_position = 0;
	}
	
	private Token Current => Peek(0);
	
	private Token Peek(int offset)
	{
		int index = _position + offset;
		if (index >= _rawTokens.Length)
			return _rawTokens[^1];

		return _rawTokens[index];
	}
	
	private Token NextToken()
	{
		Token current = Current;
		_position++;
		return current;
	}
	
	private Token MatchToken(SyntaxKind kind)
	{
		if (Current.Kind == kind)
			return NextToken();

		// _diagnostics.ReportUnexpectedToken(Current.Location, Current.Kind, kind);
		return new Token(kind, "", [], []);
	}

	private bool IsPresentedAny(params ReadOnlySpan<SyntaxKind> kinds)
	{
		return kinds.Contains(Current.Kind);
	}

	public NiteCodeCompilationUnit ParseCompilationUnit()
	{
		if (Current.Kind == UseKeyword)
		{
			ParseModuleDeclaration();
		}
		return new NiteCodeCompilationUnit();
	}

	private ModuleSyntax ParseModuleDeclaration()
	{
		Token useKeyword = MatchToken(UseKeyword);
		ModuleNameSyntax moduleName = ParseModuleName();
		return new ModuleSyntax(useKeyword, moduleName);
	}

	private ModuleNameSyntax ParseModuleName()
	{
		return new ModuleNameSyntax(identifiers: []); // TODO: Impl
	}
}