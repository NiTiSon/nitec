using System.Collections.Generic;
using System.Collections.Immutable;
using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public sealed class NiteCodeCompilationUnit : CompilationUnit
{
	public ImmutableArray<ISyntaxNode> Members { get; }
	public ImmutableArray<UseDirectiveSyntax> Usings { get; }
	
	public NiteCodeCompilationUnit(ImmutableArray<UseDirectiveSyntax> usings, ImmutableArray<ISyntaxNode> members)
	{
		Members = members;
		Usings = usings;
	}
	
	public override IEnumerable<ISyntaxNode> GetChildren()
	{
		foreach (UseDirectiveSyntax @using in Usings)
		{
			yield return @using;
		}
		
		foreach (ISyntaxNode member in Members)
		{
			yield return member;
		}
	}
}