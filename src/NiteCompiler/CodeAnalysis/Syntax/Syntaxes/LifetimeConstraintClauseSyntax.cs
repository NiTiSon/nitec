using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class LifetimeConstraintClauseSyntax : LifetimeOrGenericConstraintClauseSyntax
{
	public Token WhereKeyword { get; }
	public LifetimeSyntax Lifetime { get; }
	public Token ColonToken { get; }
	public SyntaxList<ConstraintSyntax> Constraints { get; }

	public override TextSpan Span => TextSpan.FromBounds(WhereKeyword.Span, Constraints.Span);
	public override NodeKind Kind => NodeKind.LifetimeConstraintClause;

	internal LifetimeConstraintClauseSyntax(SyntaxTree tree, Token whereKeyword, LifetimeSyntax lifetime,
		Token colonToken, SyntaxList<ConstraintSyntax> constraints)
		: base(tree)
	{
		WhereKeyword = whereKeyword;
		Lifetime = lifetime;
		ColonToken = colonToken;
		Constraints = constraints;
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return WhereKeyword;
		yield return Lifetime;
		yield return ColonToken;
	}
}