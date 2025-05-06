namespace Nlr.Compiler.CodeAnalysis.Syntax;

public interface ISyntaxNode
{
	SyntaxKind Kind { get; }
}