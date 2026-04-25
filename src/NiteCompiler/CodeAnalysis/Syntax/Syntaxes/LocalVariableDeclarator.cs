using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class LocalVariableDeclarator : SyntaxNode
{
	public SimpleNameSyntax Name { get; }
	public TypeClause? TypeClause { get; }
	public EqualsValueClause? EqualsValueClause { get; }

	public override TextSpan Span
	{
		get
		{
			SyntaxNode lastNode = EqualsValueClause ?? (SyntaxNode?)TypeClause ?? Name;
			return TextSpan.FromBounds(Name.Span, lastNode.Span);
		}
	}
	public override NodeKind Kind => NodeKind.LocalVariableDeclarator;

	internal LocalVariableDeclarator(SyntaxTree tree, SimpleNameSyntax name, TypeClause? typeClause, EqualsValueClause? equalsValueClause) : base(tree)
	{
		Name = name;
		TypeClause = typeClause;
		EqualsValueClause = equalsValueClause;
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitLocalVariableDeclarator(this);
	public override void Accept(SyntaxVisitor visitor) => visitor.VisitLocalVariableDeclarator(this);

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Name;
		if (TypeClause != null) yield return TypeClause;
		if (EqualsValueClause != null) yield return EqualsValueClause;
	}
}