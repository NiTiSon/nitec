using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class BlockFunctionBodySyntax : FunctionBodySyntax
{
	public BlockStatementSyntax Block { get; }

	public override NodeKind Kind => NodeKind.FunctionBlockBody;
	public override TextSpan Span => Block.Span;

	internal override Token ClosingToken => Block.CloseBrace;

	public BlockFunctionBodySyntax(SyntaxTree tree, BlockStatementSyntax block) : base(tree)
	{
		Block = block;
	}

	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitBlockFunctionBody(this);
	public override void Accept(SyntaxVisitor visitor) => visitor.VisitBlockFunctionBody(this);

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Block;
	}
}