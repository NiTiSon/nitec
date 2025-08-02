using System.Collections.Generic;
using NiTiS.Compiler.CodeAnalysis.Syntax;

namespace NiTiS.Compiler.NiteCode.CodeAnalysis.Syntax.Statements;

public abstract class StatementSyntax : ISyntaxNode
{
	public abstract IEnumerable<ISyntaxNode> GetChildren();
}