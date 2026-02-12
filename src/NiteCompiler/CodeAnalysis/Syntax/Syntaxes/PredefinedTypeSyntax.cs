using System.Collections.Generic;
using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class PredefinedTypeSyntax : TypeSyntax
{
	public Token TypeKeyword { get; }

	public override TextSpan Span => TypeKeyword.Span;
	public override NodeKind Kind => NodeKind.PredefinedType;

	public PredefinedTypeSyntax(SyntaxTree tree, Token typeKeyword) : base(tree)
	{
		Debug.Assert(typeKeyword.IsTypeKeyword);
		TypeKeyword = typeKeyword;
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return TypeKeyword;
	}
}