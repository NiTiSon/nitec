using System.Collections.Generic;
using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class CompilationUnitSyntax : SyntaxNode
{
	public ImmutableArray<SyntaxNode> TopLevelNodes { get; }
	public Token EndOfFileToken { get; }
	public SourceText Source { get; }

	public CompilationUnitSyntax(SourceText source, ImmutableArray<SyntaxNode> topLevelNodes, Token endOfFileToken)
	{
		Source = source;
		TopLevelNodes = topLevelNodes;
		EndOfFileToken = endOfFileToken;
	}

	public override TextSpan Span => Source.Span;
	public override SyntaxKind Kind => SyntaxKind.CompilationUnit;
	public override IEnumerable<SyntaxNode> GetChildren()
	{
		return TopLevelNodes;
	}
}