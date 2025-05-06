using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public abstract class NameSyntax : ISyntaxNode
{
	public abstract SyntaxKind Kind { get; }
}