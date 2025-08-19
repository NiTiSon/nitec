using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public abstract class SyntaxNode
{
	public abstract TextSpan Span { get; }
	public abstract SyntaxKind Kind { get; }
}