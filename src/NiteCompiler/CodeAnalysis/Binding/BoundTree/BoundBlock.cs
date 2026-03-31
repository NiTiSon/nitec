using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundBlock : BoundStatement
{
	public ImmutableArray<BoundStatement> Statements { get; }

	public override BoundKind Kind => BoundKind.Block;

	public BoundBlock(SyntaxNode syntax, ImmutableArray<BoundStatement> statements) : base(syntax)
	{
		Statements = statements;
	}

	public override void Accept(BoundVisitor visitor)
	{
		visitor.VisitBlock(this);
	}
}