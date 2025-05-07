using System.Collections.Generic;

namespace Nlr.Compiler.CodeAnalysis.Syntax;

public abstract class CompilationUnit : ISyntaxNode
{
	public SyntaxKind Kind => SyntaxKind.CompilationUnit;

	public abstract IEnumerable<ISyntaxNode> GetChildren();
}