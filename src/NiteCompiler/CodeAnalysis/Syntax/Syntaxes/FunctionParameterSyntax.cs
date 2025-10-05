using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class FunctionParameterSyntax : SyntaxNode
{
	public FunctionParameterSyntax(SimpleNameSyntax name, TypeClauseSyntax type)
	{
		Name = name;
		Type = type;
	}

	public SimpleNameSyntax Name { get; }
	public TypeClauseSyntax Type { get; }


	public override TextSpan Span => TextSpan.FromBounds(Name.Span, Type.Span);

	public override SyntaxKind Kind =>  SyntaxKind.Parameter;

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Name;
		yield return Type;
	}
}