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

	private ModuleDeclarationSyntax? _currentModule;
	
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

	private Token MatchAnyToken(params ReadOnlySpan<SyntaxKind> kinds)
	{
		if (kinds.Contains(Current.Kind))
			return NextToken();

		// _diagnostics.ReportUnexpectedToken(Current.Location, Current.Kind, kind);
		return new Token(kinds[0], "", [], []);
	}

	private bool IsPresentedAny(params ReadOnlySpan<SyntaxKind> kinds)
	{
		return kinds.Contains(Current.Kind);
	}

	public NiteCodeCompilationUnit ParseCompilationUnit()
	{
		ImmutableArray<UseDirectiveSyntax>.Builder usings = ImmutableArray.CreateBuilder<UseDirectiveSyntax>();
		
		while (Current.Kind != EndOfFile)
		{
			switch (Current.Kind)
			{
				case UnknownOrWrong:
					NextToken();
					break;
				case UseKeyword:
					usings.Add(ParseUsingDirective());
					break;
				case ModuleKeyword:
					ParseModuleDeclaration();
					break;
				case PublicKeyword
					or PrivateKeyword
					or ProtectedKeyword
					or InternalKeyword
					or FamilyKeyword
					or FriendKeyword:
					ParseMember();
					break;
				default:
					NextToken();
					break;
			}
		}
		return new NiteCodeCompilationUnit(usings.ToImmutable());
	}

	private ModuleDeclarationSyntax ParseModuleDeclaration()
	{
		Token moduleKeyword = MatchToken(ModuleKeyword);
		ModuleNameSyntax moduleName = ParseModuleName();
		return new ModuleDeclarationSyntax(moduleKeyword, moduleName);
	}
	
	private UseDirectiveSyntax ParseUsingDirective()
	{
		Token useKeyword = MatchToken(UseKeyword);
		ModuleNameSyntax moduleName = ParseModuleName();
		return new UseDirectiveSyntax(useKeyword, moduleName);
	}

	private ModuleNameSyntax ParseModuleName()
	{
		ImmutableArray<Token>.Builder tokens = ImmutableArray.CreateBuilder<Token>();
		tokens.Add(MatchToken(IdentifierToken));
		while (Current.Kind == ColonColonToken)
		{
			NextToken();
			tokens.Add(NextToken());
		}
		return new ModuleNameSyntax(tokens.ToImmutable()); // TODO: Impl
	}

	private MemberSyntax ParseMember()
	{
		Token accessLevelToken = MatchAnyToken(PublicKeyword, ProtectedKeyword, InternalKeyword, FamilyKeyword, FriendKeyword, PrivateKeyword);
		ImmutableArray<Token>.Builder modifiers = ImmutableArray.CreateBuilder<Token>();

		while (IsPresentedAny())
		{
			modifiers.Add(NextToken());
		}

		if (Current.Kind == TypeKeyword)
		{
			Token typeKeyword = MatchToken(TypeKeyword);
			Token identifier = MatchToken(IdentifierToken);
			return new TypeSyntax(accessLevelToken, modifiers.ToImmutable(), typeKeyword, identifier);
		}
		
		// [access_token] [modifiers] TypeKeyword Identifier

		// [access_token] [modifiers] Identifier ( parameter_list )

		// [access_token] [modifiers] Identifier ColonToken NameSyntax
		return null!;
	}
}