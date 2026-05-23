using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class InlineNameSyntax : NameSyntax
{
	public NameSyntax Left { get; }
	public Token DoubleColon { get; }
	public SimpleNameSyntax Right { get; }
	public LifetimeAndGenericParameterListSyntax? GenericParameterList { get; }

	public override TextSpan Span => TextSpan.FromBounds(Left.Span, GenericParameterList?.Span ?? Right.Span);
	public override NodeKind Kind => NodeKind.InlineName;

	internal InlineNameSyntax(SyntaxTree tree, NameSyntax left, Token doubleColon,
		SimpleNameSyntax right, LifetimeAndGenericParameterListSyntax? genericParameterList) : base(tree)
	{
		Left = left;
		DoubleColon = doubleColon;
		Right = right;
		GenericParameterList = genericParameterList;
	}

	public override string GetName()
	{
		return Left.GetName() + "::" + Right.GetName();
	}

	public override SimpleNameSyntax UnqualifiedName => Right;

	public override int LifetimeArity => GenericParameterList?.LifetimeArity ?? 0;
	public override int Arity => GenericParameterList?.Arity ?? 0;

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Left;
		yield return DoubleColon;
		yield return Right;
		if (GenericParameterList != null) yield return GenericParameterList;
	}
}