using System.Collections.Generic;
using System.Collections.Immutable;
using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public sealed class ReturnParameterSyntax : ISyntaxNode
{
	public Token Retusa { get; }
	public NameSyntax ReturnParameter { get; }

	public ReturnParameterSyntax(Token retusa, NameSyntax returnParameter)
	{
		Retusa = retusa;
		ReturnParameter = returnParameter;
	}
	
	public SyntaxKind Kind => SyntaxKind.ReturnParameterList;
	public IEnumerable<ISyntaxNode> GetChildren()
	{
		yield return ReturnParameter;
	}
}