using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundBlockStatement : BoundStatement
{
	public ImmutableArray<BoundStatement> Statements { get; }

	public BoundBlockStatement(SyntaxNode syntax, ImmutableArray<BoundStatement> statements) : base(syntax)
	{
		Statements = statements;
	}

	public override BoundKind Kind => BoundKind.BlockStatement;
}