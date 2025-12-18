using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Text;
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
		} while (token.Kind != SyntaxKind.EofToken);
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
	private Token MatchToken(SyntaxKind kind)
	{
		if (Current.Kind == kind)
			return PeekAndAdvance();

		_diagnostics.ReportExpectedToken(Current.Span.Contextualize(_syntaxTree), kind);
		return new Token(kind, Current.Span);
	}

	[DebuggerStepThrough]
	private bool IsPresentedAny(params ReadOnlySpan<SyntaxKind> kinds)
	{
		return kinds.Contains(Current.Kind);
	}

	[DebuggerStepThrough]
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
					MatchToken(SyntaxKind.SemicolonToken);
					break;
				case SyntaxKind.ModuleKeyword:
					membersBuilder.Add(ParseModuleDeclaration());
					MatchToken(SyntaxKind.SemicolonToken);
					break;
				default:
					if (IsPresentedAny(SyntaxFacts.AccessKeywords))
					{
						membersBuilder.Add(ParseMember());
					}
					else
					{
						_position++;
						_diagnostics.ReportUnexpectedToken(Current.Span.Contextualize(_syntaxTree), Current.Kind);
					}
					break;
			}
		}
		Token endOfFileToken = MatchToken(SyntaxKind.EofToken);

		return new CompilationUnitSyntax(_syntaxTree, membersBuilder.ToImmutable(), endOfFileToken);
	}

	private readonly ImmutableArray<Token>.Builder _modifiers = ImmutableArray.CreateBuilder<Token>(16);
	private MemberSyntax ParseMember()
	{
		Token accessibilityToken = MatchAnyToken(SyntaxFacts.AccessKeywords);

		while (IsPresentedAny(SyntaxFacts.ModifiersKeywords))
		{
			_modifiers.Add(PeekAndAdvance());
		}

		if (Current.Kind == SyntaxKind.TypeKeyword)
		{
			return ParseTypeDeclaration(accessibilityToken, _modifiers.DrainToImmutable(), PeekAndAdvance());
		}

		SimpleNameSyntax name = ParseSimpleName();

		if (Current.Kind == SyntaxKind.OpenParenToken) // Function/Method
		{
			return ParseFunctionDeclaration(accessibilityToken, _modifiers.DrainToImmutable(), name);
		}
		else // field
		{
			return ParseFieldDeclaration(accessibilityToken, _modifiers.DrainToImmutable(), name);
		}
	}

	private TypeDeclarationSyntax ParseTypeDeclaration(Token accessibilityToken, ImmutableArray<Token> modifiers, Token typeKeyword)
	{
		SimpleNameSyntax name = ParseSimpleName();

		//  Parent type handling

		Token openBraceOrSemicolon = MatchAnyToken(SyntaxKind.OpenBraceToken, SyntaxKind.SemicolonToken);

		if (openBraceOrSemicolon.Kind == SyntaxKind.SemicolonToken)
		{
			return new(accessibilityToken, modifiers, typeKeyword, name, null, [], openBraceOrSemicolon);
		}

		ImmutableArray<MemberSyntax>.Builder membersBuilder = ImmutableArray.CreateBuilder<MemberSyntax>();

		while (Current.Kind != SyntaxKind.EofToken && Current.Kind != SyntaxKind.CloseBraceToken)
		{
			membersBuilder.Add(ParseMember());
		}
		Token closeBrace = MatchToken(SyntaxKind.CloseBraceToken);

		return new(accessibilityToken, modifiers, typeKeyword, name, openBraceOrSemicolon,
			membersBuilder.ToImmutable(), closeBrace);
	}

	private FieldDeclarationSyntax ParseFieldDeclaration(Token accessibilityToken, ImmutableArray<Token> modifiers,
		SimpleNameSyntax name)
	{
		TypeClauseSyntax? typeClause = null;
		ExpressionSyntax? initializer = null;
		if (Current.Kind == SyntaxKind.ColonToken) // field: type
		{
			typeClause = ParseTypeClause();
		}

		if (Current.Kind == SyntaxKind.EqualsToken) // field = expr;
		{
			Advance();
			initializer = ParseExpression();
		}

		MatchToken(SyntaxKind.SemicolonToken);
		return new FieldDeclarationSyntax(accessibilityToken, modifiers, name, typeClause, initializer);
	}

	private FunctionDeclarationSyntax ParseFunctionDeclaration(Token accessibilityToken, ImmutableArray<Token> modifiers,
		SimpleNameSyntax name)
	{
		SyntaxList<FunctionParameterSyntax> parameters = ParseParameterList();

		RetusaClauseSyntax? retusa = null;
		if (Current.Kind == SyntaxKind.RetusaToken)
		{
			Token retusaArrow = MatchToken(SyntaxKind.RetusaToken);

			TypeSyntax returnParameter = ParseType();
			retusa = new(retusaArrow, returnParameter);
		}

		BlockStatementSyntax block = ParseBlockStatement();

		return new FunctionDeclarationSyntax(accessibilityToken, modifiers, name, parameters, retusa, block);
	}

	private SyntaxList<FunctionParameterSyntax> ParseParameterList()
	{
		SyntaxList<FunctionParameterSyntax>.Builder list = new(SyntaxKind.ParameterList);

		MatchToken(SyntaxKind.OpenParenToken);

		while (Current.Kind is not (SyntaxKind.EofToken or SyntaxKind.CloseParenToken))
		{
			list.Add(ParseParameter());
		}

		MatchToken(SyntaxKind.CloseParenToken);

		return list.Build();

		FunctionParameterSyntax ParseParameter()
		{
			SimpleNameSyntax name = ParseSimpleName();
			TypeClauseSyntax? typeClause = null;
			ExpressionSyntax? initializer = null;
			if (Current.Kind == SyntaxKind.ColonToken)
			{
				 typeClause = ParseTypeClause();
			}

			if (Current.Kind == SyntaxKind.EqualsToken)
			{
				initializer = ParseExpression();
			}

			return new(name, typeClause, initializer);
		}
	}

	private TypeClauseSyntax ParseTypeClause()
	{
		Token colon = MatchToken(SyntaxKind.ColonToken);
		TypeSyntax type = ParseType();

		return new(colon, type);
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