using System.Collections.Generic;
using System.Collections.Immutable;
using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public sealed class ParameterListSyntax : ISyntaxNode
{
	public ImmutableArray<ParameterSyntax> Parameters { get; }

	public ParameterListSyntax(ImmutableArray<ParameterSyntax> parameters)
	{
		Parameters = parameters;
	}

	public SyntaxKind Kind => SyntaxKind.ParameterList;
	public IEnumerable<ISyntaxNode> GetChildren()
	{
		return Parameters;
	}
}