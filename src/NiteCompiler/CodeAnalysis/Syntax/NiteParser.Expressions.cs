using System;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed partial class NiteParser
{
	private ExpressionSyntax ParseExpression()
	{
		return ParseSubExpression(Precedence.Expression);
	}

	private ExpressionSyntax ParseSubExpression(Precedence precedence)
	{
		ExpressionSyntax result = Impl(precedence);

		#if DEBUG
		_ = SyntaxFacts.GetPrecedence(result.Kind);
		#endif

		return result;

		ExpressionSyntax Impl(Precedence implPrecedence)
		{
			return ParseExpressionContinued(ParsePrimaryOrUnaryExpression(), implPrecedence);
		}
	}

	private ExpressionSyntax ParseExpressionContinued(ExpressionSyntax unaryOrPrimaryExpression, Precedence precedence)
	{
		ExpressionSyntax currentExpression = unaryOrPrimaryExpression;

		while (TryExpandExpression(currentExpression, precedence) is ExpressionSyntax expandedExpression)
			currentExpression = expandedExpression;

		return currentExpression;
	}

	private ExpressionSyntax? TryExpandExpression(ExpressionSyntax leftOperand, Precedence precedence)
	{
		// TODO: Check for >>> like operators

		(SyntaxKind operatorTokenKind, SyntaxKind operatorExpressionKind) = GetExpressionOperatorTokenKindAndExpressionKind();

		if (operatorTokenKind == SyntaxKind.None)
			return null;

		Precedence newPrecedence = SyntaxFacts.GetPrecedence(operatorExpressionKind);

		// Console.WriteLine($"opKind={operatorExpressionKind} newPrec={(int)newPrecedence} prec={(int)precedence} isRight={SyntaxFacts.IsRightAssociativeExpression(operatorExpressionKind)}");

		if (newPrecedence < precedence)
			return null;

		if ((newPrecedence == precedence) && !SyntaxFacts.IsRightAssociativeExpression(operatorExpressionKind))
			return null;

		// TODO: Add support for >>, >>>, >>>=
		Token operatorToken = PeekAndAdvance();

		if (newPrecedence > SyntaxFacts.GetPrecedence(operatorExpressionKind))
		{
			Console.Error.WriteLine("!!! TryExpandExpression@NiteParser.Expressions.cs !!! INVALID BEHAVIOUR");
		}

		if (SyntaxFacts.IsAssignmentExpressionOperatorToken(operatorToken.Kind))
		{
			return ParseAssignmentExpression(operatorExpressionKind, leftOperand, operatorToken);
		}
		if (SyntaxFacts.IsBinaryExpressionOperatorToken(operatorToken.Kind))
		{
			return new BinaryExpressionSyntax(leftOperand, operatorToken, ParseSubExpression(newPrecedence), operatorExpressionKind);
		}

		throw new Exception("Unreachable");
	}

	private AssignmentExpressionSyntax ParseAssignmentExpression(SyntaxKind operatorExpressionKind,
		ExpressionSyntax leftOperand, Token operatorToken)
	{
		ExpressionSyntax rhs = ParseSubExpression(Precedence.Assignment);

		return new(leftOperand, operatorToken, rhs, operatorExpressionKind);
	}

	private ExpressionSyntax ParsePrimaryExpression()
	{
		SyntaxKind literalType;
		if ((literalType = SyntaxFacts.GetLiteralExpression(Current.Kind)) != SyntaxKind.None)
		{
			return new LiteralExpressionSyntax(PeekAndAdvance(), literalType);
		}

		if (Current.Kind == SyntaxKind.OpenParenToken)
		{
			return ParseParenthesizedExpression();
		}

		throw new NotImplementedException();
	}

	private ExpressionSyntax ParsePrimaryOrUnaryExpression()
	{
		if (SyntaxFacts.IsUnaryExpression(Current.Kind))
		{
			Token operatorToken = Current;
			SyntaxKind opKind = SyntaxFacts.GetUnaryExpression(Current.Kind);
			ExpressionSyntax expression = ParseSubExpression(SyntaxFacts.GetPrecedence(opKind));

			return new UnaryExpressionSyntax(operatorToken, expression, opKind);
		}

		return ParsePrimaryExpression();
	}

	private ParenthesizedExpressionSyntax ParseParenthesizedExpression()
	{
		Token openParen = MatchToken(SyntaxKind.OpenParenToken);

		ExpressionSyntax expression = ParseExpression();

		Token closeParen = MatchToken(SyntaxKind.CloseParenToken);

		return new(openParen, expression, closeParen);
	}


	private TypeSyntax ParseType()
	{
		if (SyntaxFacts.IsTypeKeyword(Current.Kind))
		{
			return new PredefinedTypeSyntax(PeekAndAdvance());
		}

		return ParseName();
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

			NameSyntax nameSyntax = ParseSimpleName(); // Replace with simple name for generics support.

			return new NameWithExplicitModuleSyntax(moduleName!, colonColonToken, nameSyntax);
		}
		else
		{
			return ParseSimpleName();
		}
	}

	private ModuleNameSyntax ParseModuleName()
	{
		SyntaxList<SimpleNameSyntax>.Builder parts = new(SyntaxKind.IdentifierList);

		parts.Add(ParseSimpleName());

		while (Current.Kind == SyntaxKind.ColonColonToken)
		{
			Token next = Peek(1);
			if (next.Kind == SyntaxKind.IdentifierToken && Peek(2).Kind == SyntaxKind.ColonColonToken)
			{
				Advance(); // ::
				parts.Add(ParseSimpleName());
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
	private ModuleNameSyntax ParseModuleNameAlone()
	{
		SyntaxList<SimpleNameSyntax>.Builder parts = new(SyntaxKind.IdentifierList);

		parts.Add(ParseSimpleName());

		while (Current.Kind == SyntaxKind.ColonColonToken)
		{
			Advance();
			if (Current.Kind == SyntaxKind.IdentifierToken)
			{
				parts.Add(ParseSimpleName());
			}
			else
			{
				// _diagnostics.Report
				break;
			}
		}

		return new ModuleNameSyntax(parts.Build());
	}

	private SimpleNameSyntax ParseSimpleName()
	{
		return new((MatchToken(SyntaxKind.IdentifierToken) as IdentifierToken)!);
	}
}