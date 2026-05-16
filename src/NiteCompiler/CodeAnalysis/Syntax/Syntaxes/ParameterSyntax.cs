using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class ParameterSyntax : BaseParameterSyntax
{
	public SimpleNameSyntax Name { get; }
	public TypeClauseSyntax TypeClauseSyntax { get; }
	public override NodeKind Kind => NodeKind.Parameter;
	public override TextSpan Span => TextSpan.FromBounds(Name.Span, TypeClauseSyntax.Span);

	internal ParameterSyntax(SyntaxTree tree, SimpleNameSyntax name, TypeClauseSyntax typeClauseSyntax) : base(tree)
	{
		Name = name;
		TypeClauseSyntax = typeClauseSyntax;
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
		yield return TypeClauseSyntax;
	}
}