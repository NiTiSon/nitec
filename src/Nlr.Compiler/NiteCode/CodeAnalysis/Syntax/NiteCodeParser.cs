using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
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

	private readonly ImmutableArray<ISyntaxNode>.Builder _members;

	public NiteCodeParser(DiagnosticBag diagnostics, NiteCodeLexer lexer)
	{
		_members = ImmutableArray.CreateBuilder<ISyntaxNode>();
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

	private Token Current
	{
		[DebuggerStepperBoundary]
		get => Peek(0);
	}
	
	[DebuggerStepperBoundary]
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
					_members.Add(ParseModuleDeclaration());
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
		return new ModuleNameSyntax(tokens.ToImmutable());
	}

	private NameSyntax ParseName()
	{
		// i32 - PredefinedTypeNameSyntax
		
		// ::module::name.Type.SubType - QualifiedTypeNameSyntax
		// ::module::name - module part
		// Type.SubType
		
		if (SyntaxFacts.IsTypeKeyword(Current.Kind))
		{
			return new PredefinedTypeNameSyntax(NextToken());
		}

		ModuleNameSyntax? moduleName = null;
		if (Current.Kind == ColonColonToken) // ::module::name
		{
			NextToken(); // Skip `::`
			moduleName = ParseModuleName();
			MatchToken(DotToken);
		}

		NameSyntax left = ParseSimpleName();
		while (Current.Kind == DotToken)
		{
			Token dot = NextToken();

			var right = ParseSimpleName();
			left = new QualifiedTypeNameSyntax(left, dot, right);
		}

		if (moduleName != null)
		{
			left = new FullyQualifiedNameSyntax(moduleName, left);
		}

		return left;
	}

	private SimpleNameSyntax ParseSimpleName()
	{
		return new IdentifierNameSyntax(MatchToken(IdentifierToken));
	}

	private MemberSyntax ParseMember()
	{
		// [access_token] [modifiers] TypeKeyword Identifier

		// [access_token] [modifiers] Identifier ( parameter_list )

		// [access_token] [modifiers] Identifier ColonToken NameSyntax
		
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
			SimpleNameSyntax identifier = ParseSimpleName();
			switch (Current.Kind)
			{
				// function
				case OpenParenToken:
					MatchToken(OpenParenToken);
					ParameterListSyntax parameters = ParseParameters(aPosterioriEmpty: Current.Kind == CloseParenToken);
					MatchToken(CloseParenToken);

					ReturnParameterSyntax? returnParameter = null;
					if (Current.Kind == MinusGreaterThanToken) // Return type
					{
						Token retusa = NextToken();
						NameSyntax returnType = ParseName();
						returnParameter = new ReturnParameterSyntax(retusa, returnType);
					}

					BlockSyntax body = ParseBlock();

					return new FunctionSyntax(accessLevelToken, modifiers.ToImmutable(), identifier, parameters, body, returnParameter);
				// field
				case ColonToken:
					break;
				default:
					break;
			}
		}
		
		IncompleteMemberSyntax incompleteMember = new(accessLevelToken, modifiers.ToImmutable());
		_diagnostics.ReportIncompleteMember(incompleteMember);
		return incompleteMember;
	}

	private ParameterListSyntax ParseParameters(bool aPosterioriEmpty = false)
	{
		if (aPosterioriEmpty) return new ParameterListSyntax([]);
		
		ImmutableArray<ParameterSyntax>.Builder parameters = ImmutableArray.CreateBuilder<ParameterSyntax>();
		do
		{
			parameters.Add(ParseParameter());

			if (Current.Kind == CommaToken)
			{
				NextToken();
			}
			else
			{
				break;
			}
		}
		while (true);
		
		return new ParameterListSyntax(parameters.ToImmutable());
	}

	private ParameterSyntax ParseParameter()
	{
		SimpleNameSyntax name = ParseSimpleName();
		MatchToken(ColonToken);
		NameSyntax type = ParseName();
		return new ParameterSyntax(name, type);
	}

	private StatementSyntax ParseStatement()
	{
		switch (Current.Kind)
		{
			case LetKeyword:
				return ParseLocalVariableSyntax();
			case OpenBraceToken: // Block statement
				return ParseBlock();
			case ReturnKeyword: // return
				Token returnKeyword = NextToken();
				if (Current.Kind == SemicolonToken)
				{
					return new ReturnStatement(returnKeyword);
				}
				
				return new ReturnStatement(returnKeyword, ParseExpression());
			case LoopKeyword:
				Token loopKeyword = NextToken();
				StatementSyntax body = ParseStatement();
				return new LoopStatementSyntax(loopKeyword, body);
			case SemicolonToken:
				return new EmptyStatementSyntax();
			default:
				return new ExpressionStatementSyntax(ParseExpression());
		}
	}

	private LocalVariableDeclarationSyntax ParseLocalVariableSyntax()
	{
		Token letKeword = MatchToken(LetKeyword);
		SimpleNameSyntax name = ParseSimpleName();
		if (Current.Kind != ColonToken)
			return new LocalVariableDeclarationSyntax(letKeword, name);
		
		NextToken();
	
		NameSyntax type = ParseName();

		return new LocalVariableDeclarationSyntax(letKeword, name, type);

	}

	private BlockSyntax ParseBlock()
	{
		Token openBraceToken = MatchToken(OpenBraceToken);

		ImmutableArray<StatementSyntax>.Builder statements = ImmutableArray.CreateBuilder<StatementSyntax>();
		while (true)
		{
			if (Current.Kind == CloseBraceToken)
			{
				break;
			}

			if (Current.Kind is EndOfFile)
			{
				// _diagnostics.ReportSomething
				break;
			}

			StatementSyntax statement = ParseStatement();
			statements.Add(statement);

			if (statement is not IScopeDefyingStatement)
			{
				MatchToken(SemicolonToken);
			}
		}
		Token closeBraceToken = NextToken();
		
		return new BlockSyntax(openBraceToken, statements.ToImmutable(), closeBraceToken);
	}

	private ExpressionSyntax ParseExpression()
	{
		return ParseBinaryExpression();
	}
	
	private ExpressionSyntax ParseBinaryExpression(int parentPrecedence = 0)
	{
		ExpressionSyntax left;
		int unaryOperatorPrecedence = SyntaxFacts.GetUnaryPrecedence(Current.Kind);

		if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecedence)
		{
			Token operatorToken = NextToken();
			ExpressionSyntax operand = ParseBinaryExpression(unaryOperatorPrecedence);
			left = new UnaryExpressionSyntax(operatorToken, operand);
		}
		else
		{
			left = ParsePrimaryExpression();
		}

		while (true)
		{
			int precedence = SyntaxFacts.GetPrecedence(Current.Kind);
			if (precedence == 0 || precedence <= parentPrecedence)
				break;

			Token operatorToken = NextToken();
			ExpressionSyntax right = ParseBinaryExpression(precedence);
			left = new BinaryExpressionSyntax(left, operatorToken, right);
		}

		return left;
	}

	private ExpressionSyntax ParsePrimaryExpression()
	{
		switch (Current.Kind)
		{
			case ColonColonToken:
			case IdentifierToken:
				return ParseName();
			case FalseKeyword:
			case TrueKeyword:
			case NumberToken:
				Token literal = NextToken();
				
				return new LiteralExpressionSyntax(literal);
			case OpenParenToken:
				Token openParenToken = NextToken();

				ExpressionSyntax expression = ParseExpression();

				Token closeParenToken = MatchToken(CloseParenToken);

				return new ParenthesizeExpressionSyntax(openParenToken, expression, closeParenToken);
		}

		_diagnostics.ReportExceptedExpression();
		_ = NextToken();
		return new WrongExpressionSyntax();
	}
}