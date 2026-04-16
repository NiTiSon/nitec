using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Compilation;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BlockBinder : LocalScopeBinder
{
	private readonly BlockStatementSyntax _block;

	public BlockBinder(Binder parent, BlockStatementSyntax block) : base(parent)
	{
		_block = block;
	}
}