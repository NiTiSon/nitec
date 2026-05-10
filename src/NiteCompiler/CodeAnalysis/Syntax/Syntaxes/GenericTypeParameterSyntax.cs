using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class GenericTypeParameterSyntax : GenericParameterSyntax
{
	public SimpleNameSyntax Name { get; }

	public override NodeKind Kind => NodeKind.GenericTypeParameter;
	public override TextSpan Span =>  TextSpan.FromBounds(Name.Span, Name.Span);

	internal GenericTypeParameterSyntax(SyntaxTree tree, SimpleNameSyntax name) : base(tree)
	{
		Name = name;
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Name;
	}
}