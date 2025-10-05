using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class VariableDeclarationSyntax : StatementSyntax
{
	public Token LetKeyword { get; }
	public SimpleNameSyntax Name { get; }
	public TypeClauseSyntax? Type { get; }
	public ExpressionSyntax? Initializer { get; }

	public VariableDeclarationSyntax(Token letKeyword, SimpleNameSyntax name, TypeClauseSyntax? typeClause,
		ExpressionSyntax? initializer)
	{
		LetKeyword = letKeyword;
		Name = name;
		Type = typeClause;
		Initializer = initializer;
	}

	public override TextSpan Span =>
		TextSpan.FromBounds(
			LetKeyword.Span,
			Initializer?.Span ?? Type?.Span ?? Name.Span);

	public override SyntaxKind Kind => SyntaxKind.VariableDeclaration;
	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return LetKeyword;
		yield return Name;
		if (Type != null) yield return Type;
		if (Initializer != null) yield return Initializer;
	}
}