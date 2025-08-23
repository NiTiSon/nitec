using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Syntax.Directives;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class SyntaxTree
{
	public readonly ImmutableArray<UseDirectiveSyntax> Usages;

	internal SyntaxTree(ImmutableArray<UseDirectiveSyntax> usages)
	{
		Usages = usages;
	}
}