using System.Collections.Generic;
using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class CompilationUnitSyntax : SyntaxNode
{
	public ImmutableArray<SyntaxNode> TopLevelNodes { get; }
	public Token EndOfFileToken { get; }

	public CompilationUnitSyntax(SyntaxTree owner, ImmutableArray<SyntaxNode> topLevelNodes, Token endOfFileToken) : base(owner)
	{
		TopLevelNodes = topLevelNodes;
		EndOfFileToken = endOfFileToken;
	}

	public override TextSpan Span => Tree.Text.Span;
	public override NodeKind Kind => NodeKind.CompilationUnit;
	public override IEnumerable<SyntaxNode> GetChildren()
	{
		return TopLevelNodes;
	}
}