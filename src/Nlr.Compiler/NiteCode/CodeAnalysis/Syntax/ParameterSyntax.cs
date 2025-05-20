using System.Collections.Generic;
using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public class ParameterSyntax : ISyntaxNode
{
	public SimpleNameSyntax ParameterName { get; }
	public NameSyntax ParameterType { get; }

	public ParameterSyntax(SimpleNameSyntax parameterName, NameSyntax parameterType)
	{
		ParameterName = parameterName;
		ParameterType = parameterType;
	}

	public SyntaxKind Kind => SyntaxKind.ParameterDeclaration;
	public IEnumerable<ISyntaxNode> GetChildren()
	{
		yield return ParameterName;
		yield return ParameterType;
	}
}