using System.Collections.Generic;
using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class AttributeSyntax : SyntaxNode
{
	public NameSyntax Name { get; }
	public ArgumentListSyntax? ArgumentList { get; }

	public override TextSpan Span
	{
		get
		{
			if (ArgumentList != null)
			{
				return TextSpan.FromBounds(Name.Span, ArgumentList.Span);
			}

			return Name.Span;
		}
	}

	public override NodeKind Kind => NodeKind.Attribute;

	internal AttributeSyntax(SyntaxTree tree, NameSyntax name,
		ArgumentListSyntax? argumentList) : base(tree)
	{
		Name = name;
		ArgumentList = argumentList;
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default =>
		visitor.VisitAttribute(this);

	public override void Accept(SyntaxVisitor visitor) =>
		visitor.VisitAttribute(this);

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Name;
		if (ArgumentList != null) yield return ArgumentList;
	}
}
