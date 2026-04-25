using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class PathNameSyntax : NameSyntax
{
	public NameSyntax Left { get; }
	public Token DoubleColon { get; }
	public SimpleNameSyntax Right { get; }

	public override NodeKind Kind => NodeKind.PathNameExpression;
	public override TextSpan Span => TextSpan.FromBounds(Left.Span, Right.Span);

	internal PathNameSyntax(SyntaxTree tree, NameSyntax left, Token doubleColon, SimpleNameSyntax right) : base(tree)
	{
		Left = left;
		DoubleColon = doubleColon;
		Right = right;
	}

	public override string GetName()
	{
		return Left.GetName() + "::" + Right.GetName();
	}

	public override SimpleNameSyntax UnqualifiedName => Right;

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Left;
		yield return DoubleColon;
		yield return Right;
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitPathName(this);
	public override void Accept(SyntaxVisitor visitor) => visitor.VisitPathName(this);
}