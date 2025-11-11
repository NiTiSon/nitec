using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal abstract class BoundStatement : BoundNode
{
	protected BoundStatement(SyntaxNode syntax) : base(syntax)
	{
	}
}