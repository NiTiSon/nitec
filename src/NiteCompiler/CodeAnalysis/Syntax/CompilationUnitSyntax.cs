using System.Collections.Generic;
using System.Linq;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class CompilationUnitSyntax : SyntaxNode
{
	public SyntaxList<ItemSyntax> Items { get; }
	public Token EndOfFileToken { get; }

	public CompilationUnitSyntax(SyntaxTree owner, SyntaxList<ItemSyntax> items, Token endOfFileToken) : base(owner)
	{
		Items = items;
		EndOfFileToken = endOfFileToken;
	}

	public override TextSpan Span => Tree.Text.Span;
	public override NodeKind Kind => NodeKind.CompilationUnit;

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitCompilationUnit(this);
	public override void Accept(SyntaxVisitor visitor) => visitor.VisitCompilationUnit(this);

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		return Items;
	}
}