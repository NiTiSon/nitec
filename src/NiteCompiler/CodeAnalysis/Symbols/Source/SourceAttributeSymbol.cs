using System;
using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceAttributeSymbol : AttributeSymbol
{
	public AttributeDeclarationSyntax Syntax { get; }

	public SourceAttributeSymbol(Symbol containingSymbol, AttributeDeclarationSyntax syntax)
		: base(syntax.Name.GetName(), containingSymbol,
			parameters: ImmutableArray<ParameterSymbol>.Empty,
			targets: ParseTargets(syntax))
	{
		Syntax = syntax;
	}

	private static AttributeTargets ParseTargets(AttributeDeclarationSyntax syntax)
	{
		if (syntax.Targets == null || syntax.Targets.Count == 0)
		{
			return AttributeTargets.All;
		}

		AttributeTargets targets = AttributeTargets.None;
		foreach (Token targetToken in syntax.Targets)
		{
			string? targetName = GetTokenIdentifier(targetToken);
			targets |= targetName switch
			{
				"function" => AttributeTargets.Function,
				"type" => AttributeTargets.Type,
				"field" => AttributeTargets.Field,
				"constructor" => AttributeTargets.Constructor,
				"attribute" => AttributeTargets.Attribute,
				_ => AttributeTargets.None,
			};
		}

		return targets;
	}

	private static string? GetTokenIdentifier(Token token)
	{
		if (token is IdentifierOrKeywordToken idToken)
		{
			return idToken.Identifier;
		}

		return null;
	}
}
