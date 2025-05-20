using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public sealed class FunctionSyntax : MemberSyntax
{
	public SimpleNameSyntax Name { get; }

	public ParameterListSyntax Parameters { get; }

	public BlockSyntax Body { get; }

	public ReturnParameterSyntax? ReturnParameter { get; }

	public FunctionSyntax(Token accessToken, ImmutableArray<Token> modifiers, SimpleNameSyntax name, ParameterListSyntax parameters, BlockSyntax body, [Optional] ReturnParameterSyntax? returnParameter) : base(accessToken, modifiers)
	{
		Name = name;
		Body = body;
		Parameters = parameters;
		ReturnParameter = returnParameter;
	}
	public override SyntaxKind Kind => SyntaxKind.FunctionDeclaration;

	public override IEnumerable<ISyntaxNode> GetChildren()
	{
		yield return Name;
		yield return Parameters;
		yield return Body;
		if (ReturnParameter != null)
		{
			yield return ReturnParameter;
		}
	}
}