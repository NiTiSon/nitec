using System.Collections.Generic;
using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class CompilationUnitSyntax : SyntaxNode
{
	public ImmutableArray<SyntaxNode> TopLevelNodes { get; }
	public Token EndOfFileToken { get; }

	public CompilationUnitSyntax(SyntaxTree owner, ImmutableArray<SyntaxNode> topLevelNodes, Token endOfFileToken)
	{
		SyntaxTree = owner;
		TopLevelNodes = topLevelNodes;
		EndOfFileToken = endOfFileToken;
	}

	public override TextSpan Span => SyntaxTree.Text.Span;
	public override SyntaxKind Kind => SyntaxKind.CompilationUnit;
	public override IEnumerable<SyntaxNode> GetChildren()
	{
		return TopLevelNodes;
	}
}