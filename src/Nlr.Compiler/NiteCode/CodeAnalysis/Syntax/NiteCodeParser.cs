using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Nlr.Compiler.CodeAnalysis.Syntax;
using Nlr.Compiler.Diagnostics;
using Nlr.Compiler.Extensions;
using static Nlr.Compiler.CodeAnalysis.Syntax.SyntaxKind;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public sealed class NiteCodeParser
{
	private readonly DiagnosticBag _diagnostics;
	private readonly ImmutableArray<Token> _rawTokens;

	private int _position;

	private ModuleDeclarationSyntax? _currentModule;
	private ImmutableArray<MemberSyntax>.Builder _members;

	public NiteCodeParser(DiagnosticBag diagnostics, NiteCodeLexer lexer)
	{
		_members = ImmutableArray.CreateBuilder<MemberSyntax>();
		_diagnostics = diagnostics;
		ImmutableArray<Token>.Builder tokens = ImmutableArray.CreateBuilder<Token>();
		ImmutableArray<Token>.Builder badTokens = ImmutableArray.CreateBuilder<Token>();
		Token token;
		do
		{
			token = lexer.Lex();

			if (token.Kind is UnknownOrWrong)
			{
				badTokens.Add(token);
			}
			else if (token.Kind is EndOfFile)
			{
				tokens.Add(token);
				break;
			}
			else
			{
				tokens.Add(token);
			}
		}
		while (true);
		
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

		_diagnostics.ReportUnexpectedToken(Current, kind);
		return new Token(kind, Current.Span, string.Empty, [], []);
	}

	private Token MatchAnyToken(params ReadOnlySpan<SyntaxKind> kinds)
	{
		if (kinds.Contains(Current.Kind))
			return NextToken();

		_diagnostics.ReportUnexpectedToken(Current, kinds.ToArray());
		return new Token(kinds[0], Current.Span, string.Empty, [], []);
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
					_currentModule = ParseModuleDeclaration();
					break;
				case PublicKeyword
					or PrivateKeyword
					or ProtectedKeyword
					or InternalKeyword
					or FamilyKeyword
					or FriendKeyword:
					_members.Add(ParseMember());
					break;
				default:
					NextToken();
					break;
			}
		}
		
		return new NiteCodeCompilationUnit(usings.ToImmutable(), _members.ToImmutableArray());
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

		if (Current.Kind == TypeKeyword) // type
		{
			Token typeKeyword = MatchToken(TypeKeyword);
			Token identifier = MatchToken(IdentifierToken);
			return new TypeSyntax(accessLevelToken, modifiers.ToImmutable(), typeKeyword, identifier);
		}
		else
		{
			Token identifier = MatchToken(IdentifierToken);
			switch (Current.Kind)
			{
				// function
				case OpenParenToken:
					Token openParenToken = MatchToken(OpenParenToken);
					Token closeParenToken = MatchToken(CloseParenToken);
					
					
					// if (Current.Kind == MinusGreaterThanToken) // Return type
					// {
					// 	Token retusa = MatchToken(MinusGreaterThanToken);
					// }
					break;
				// field
				case ColonToken:
					break;
				default:
					break;
			}
		}
		// [access_token] [modifiers] TypeKeyword Identifier

		// [access_token] [modifiers] Identifier ( parameter_list )

		// [access_token] [modifiers] Identifier ColonToken NameSyntax
		return new IncompleteMemberSyntax(accessLevelToken, modifiers.ToImmutable());
	}
}