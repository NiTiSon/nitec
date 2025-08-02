using System.Collections.Generic;
using NiTiS.Compiler.CodeAnalysis.Syntax;

namespace NiTiS.Compiler.NiteCode.CodeAnalysis.Syntax.Expressions;

public abstract class ExpressionSyntax : ISyntaxNode
{
	public abstract IEnumerable<ISyntaxNode> GetChildren();
}