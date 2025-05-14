using System.Collections.Generic;
using System.Collections.Immutable;
using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public sealed class ReturnParameterListSyntax : ISyntaxNode
{
	public ImmutableArray<NameSyntax> ReturnParameters { get; }

	public ReturnParameterListSyntax(ImmutableArray<NameSyntax> returnParameters)
	{
		ReturnParameters = returnParameters;
	}
	
	public SyntaxKind Kind => SyntaxKind.ReturnParameterList;
	public IEnumerable<ISyntaxNode> GetChildren()
	{
		return ReturnParameters;
	}
}