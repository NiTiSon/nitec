using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class LoopBinder : BaseLoopBinder
{
	private readonly LoopStatementSyntax _syntax;

	public LoopBinder(Binder parent, LoopStatementSyntax syntax) : base(parent)
	{
		_syntax = syntax;
	}
}