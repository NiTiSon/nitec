using System;
using NiteCompiler.CodeAnalysis.Syntax.Expressions;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed partial class NiteParser
{
	private ExpressionSyntax ParseExpression()
	{
		if (Current.Kind == SyntaxKind.NumberToken)
		{
			return new LiteralExpressionSyntax(PeekAndAdvance(), SyntaxKind.NumericLiteralExpression);
		}
		else
		{
			throw new NotImplementedException();
		}
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
	private ModuleNameSyntax ParseModuleNameInUseDirective()
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