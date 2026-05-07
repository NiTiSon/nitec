using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class BlockFunctionBodySyntax : FunctionBodySyntax
{
	public BlockStatementSyntax Block { get; }

	public override NodeKind Kind => NodeKind.FunctionBlockBody;
	public override TextSpan Span => Block.Span;

	internal override Token ClosingToken => Block.CloseBrace;

	internal BlockFunctionBodySyntax(SyntaxTree tree, BlockStatementSyntax block) : base(tree)
	{
		Block = block;
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Block;
	}
}