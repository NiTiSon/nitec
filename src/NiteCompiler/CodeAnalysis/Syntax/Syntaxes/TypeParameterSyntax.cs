using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class TypeParameterSyntax : GenericParameterSyntax
{
	public SimpleNameSyntax Name { get; }

	public override NodeKind Kind => NodeKind.GenericTypeParameter;
	public override TextSpan Span =>  TextSpan.FromBounds(Name.Span, Name.Span);

	internal TypeParameterSyntax(SyntaxTree tree, SimpleNameSyntax name) : base(tree)
	{
		Name = name;
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Name;
	}

	public override void Accept(SyntaxVisitor visitor)
	{
		visitor.VisitTypeParameter(this);
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default
	{
		return visitor.VisitTypeParameter(this);
	}
}