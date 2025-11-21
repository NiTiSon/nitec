using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BoundCompilationUnit : BoundNode
{
	public override BoundKind Kind => BoundKind.CompilationUnit;
	public ImmutableArray<BoundNode> Members { get; }

	public BoundCompilationUnit(CompilationUnitSyntax syntax, ImmutableArray<BoundNode> members) : base(syntax)
	{
		Members = members;
	}
}