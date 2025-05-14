using System.Collections.Generic;
using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public abstract class ExpressionSyntax : ISyntaxNode
{
	public abstract SyntaxKind Kind { get; }

	public abstract IEnumerable<ISyntaxNode> GetChildren();
}