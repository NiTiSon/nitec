using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class LifetimeConstraintSyntax : ConstraintSyntax
{
	public LifetimeSyntax Outlives { get; }

	public override NodeKind Kind => NodeKind.LifetimeConstraint;
	public override TextSpan Span => Outlives.Span;

	public LifetimeConstraintSyntax(SyntaxTree tree, LifetimeSyntax outlives) : base(tree)
	{
		Outlives = outlives;
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Outlives;
	}
}