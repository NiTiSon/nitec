using System.Collections.Immutable;
using System.Linq;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundBlock : BoundStatement
{
	public ImmutableArray<LocalVariableSymbol> Locals { get; }
	public ImmutableArray<BoundStatement> Statements { get; }

	public override BoundKind Kind => BoundKind.Block;

	public BoundBlock(SyntaxNode syntax, ImmutableArray<LocalVariableSymbol> locals, ImmutableArray<BoundStatement> statements, bool hasErrors = false)
		: base(syntax, hasErrors || statements.Any(t => t.HasErrors))
	{
		Locals = locals;
		Statements = statements;
	}

	public override void Accept(BoundVisitor visitor)
	{
		visitor.VisitBlock(this);
	}
}