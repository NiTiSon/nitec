using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal abstract class BoundStatement : BoundNode
{
	protected BoundStatement(SyntaxNode syntax, bool hasErrors = false) : base(syntax, hasErrors)
	{
	}

	public bool CompilerGenerated { get; init; }
}