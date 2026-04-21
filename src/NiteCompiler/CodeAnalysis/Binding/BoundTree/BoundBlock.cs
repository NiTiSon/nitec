using System.Collections.Immutable;
using System.Linq;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundBlock : BoundStatement
{
	public ImmutableArray<BoundStatement> Statements { get; }

	public override BoundKind Kind => BoundKind.Block;

	public BoundBlock(SyntaxNode syntax, ImmutableArray<BoundStatement> statements, bool hasErrors = false)
		: base(syntax, hasErrors || statements.Any(t => t.HasErrors))
	{
		Statements = statements;
	}

	public override void Accept(BoundVisitor visitor)
	{
		visitor.VisitBlock(this);
	}
}