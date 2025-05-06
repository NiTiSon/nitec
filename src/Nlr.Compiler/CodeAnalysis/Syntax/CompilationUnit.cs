namespace Nlr.Compiler.CodeAnalysis.Syntax;

public abstract class CompilationUnit : ISyntaxNode
{
	public SyntaxKind Kind => SyntaxKind.CompilationUnit;
}