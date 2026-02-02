using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class MembersTypeBodySyntax : TypeBodySyntax
{
	public Token OpenBrace { get; }
	public SyntaxList<ItemSyntax> Items { get; }
	public Token CloseBrace { get; }

	public MembersTypeBodySyntax(SyntaxTree tree, Token openBrace, SyntaxList<ItemSyntax> items, Token closeBrace) : base(tree)
	{
		OpenBrace = openBrace;
		Items = items;
		CloseBrace = closeBrace;
	}

	public override TextSpan Span => TextSpan.FromBounds(OpenBrace.Span, CloseBrace.Span);
	public override NodeKind Kind => NodeKind.TypeBody;
	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return OpenBrace;
		foreach (ItemSyntax item in Items)
		{
			yield return item;
		}
		yield return CloseBrace;
	}
}