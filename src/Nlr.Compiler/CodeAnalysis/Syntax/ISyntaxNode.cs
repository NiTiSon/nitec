using System.Collections.Generic;

namespace Nlr.Compiler.CodeAnalysis.Syntax;

public interface ISyntaxNode
{
	SyntaxKind Kind { get; }

	IEnumerable<ISyntaxNode> GetChildren();
}