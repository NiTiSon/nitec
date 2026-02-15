using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class CompilationUnitSyntax : SyntaxNode
{
	public SyntaxList<TopLevelSyntax> TopLevelNodes { get; }
	public Token EndOfFileToken { get; }

	public CompilationUnitSyntax(SyntaxTree owner, SyntaxList<TopLevelSyntax> topLevelNodes, Token endOfFileToken) : base(owner)
	{
		TopLevelNodes = topLevelNodes;
		EndOfFileToken = endOfFileToken;
	}

	public override TextSpan Span => Tree.Text.Span;
	public override NodeKind Kind => NodeKind.CompilationUnit;

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitCompilationUnit(this);
	public override void Accept(SyntaxVisitor visitor) => visitor.VisitCompilationUnit(this);

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		return TopLevelNodes;
	}
}