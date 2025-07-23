using System.Collections.Generic;

namespace NiTiS.Compiler.CodeAnalysis.Syntax;

public interface ISyntaxNode
{
	public IEnumerable<ISyntaxNode> GetChildren();
}