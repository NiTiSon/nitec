using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Syntax.Expressions;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public abstract class TypeSyntax : ExpressionSyntax;

public sealed class PredefinedTypeSyntax : TypeSyntax
{
	public Token Keyword { get; }

	public PredefinedTypeSyntax(Token keyword)
	{
		Keyword = keyword;
	}

	public override TextSpan Span => Keyword.Span;
	public override SyntaxKind Kind =>  SyntaxKind.PredefinedType;

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Keyword;
	}
}