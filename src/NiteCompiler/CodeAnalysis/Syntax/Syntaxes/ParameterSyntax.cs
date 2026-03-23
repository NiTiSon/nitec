using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class ParameterSyntax : SyntaxNode
{
	public SimpleNameSyntax Name { get; }
	public TypeClause TypeClause { get; }
	public override NodeKind Kind => NodeKind.Parameter;
	public override TextSpan Span => TextSpan.FromBounds(Name.Span, TypeClause.Span);

	internal ParameterSyntax(SyntaxTree tree, SimpleNameSyntax name, TypeClause typeClause) : base(tree)
	{
		Name = name;
		TypeClause = typeClause;
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default
	{
		return visitor.VisitParameter(this);
	}

	public override void Accept(SyntaxVisitor visitor)
	{
		visitor.VisitParameter(this);
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Name;
		yield return TypeClause;
	}
}