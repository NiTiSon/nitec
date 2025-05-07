using System.Collections.Generic;
using System.Collections.Immutable;
using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public sealed class NiteCodeCompilationUnit : CompilationUnit
{
	public readonly ImmutableArray<UseDirectiveSyntax> Usings;
	
	public NiteCodeCompilationUnit(ImmutableArray<UseDirectiveSyntax> usings)
	{
		Usings = usings;
	}
	
	public override IEnumerable<ISyntaxNode> GetChildren()
	{
		foreach (UseDirectiveSyntax @using in Usings)
		{
			yield return @using;
		}
	}
}