using System.Collections.Generic;

namespace NiTiS.Compiler.CodeAnalysis.Syntax;

public abstract class CompilationUnit : ISyntaxNode
{
	public abstract IEnumerable<ISyntaxNode> GetChildren();
}