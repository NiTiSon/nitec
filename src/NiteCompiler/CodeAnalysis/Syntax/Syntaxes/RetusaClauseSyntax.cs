using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

/// <summary>
/// Retusa is <c>-></c> symbol, actually just arrow. Retusa is the name for return parameter uniquely in Nite.
/// </summary>
public sealed class RetusaClauseSyntax : SyntaxNode
{
	public Token Retusa { get; }
	public TypeSyntax ReturnType { get; }

	public RetusaClauseSyntax(Token retusa, TypeSyntax returnType)
	{
		Retusa = retusa;
		ReturnType = returnType;
	}

	public override TextSpan Span => TextSpan.FromBounds(Retusa.Span, ReturnType.Span);
	public override SyntaxKind Kind => SyntaxKind.RetusaClause;

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Retusa;
		yield return ReturnType;
	}
}