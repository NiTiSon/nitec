using System.Collections.Generic;
using NiTiS.Compiler.CodeAnalysis.Syntax;

namespace NiTiS.Compiler.NiteCode.CodeAnalysis.Syntax;

public sealed class NiteCompilationUnit : ISyntaxNode
{
	public IEnumerable<ISyntaxNode> GetChildren()
	{
		return [];
	}
}