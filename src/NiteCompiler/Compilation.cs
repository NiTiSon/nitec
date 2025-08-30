using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler;

public sealed class Compilation
{
	public ImmutableArray<SyntaxTree> SyntaxTrees { get; }

	private Compilation(params SyntaxTree[] trees)
	{
		SyntaxTrees = [..trees];
	}

	public static Compilation Create(params SyntaxTree[] trees)
	{
		return new(trees);
	}
}