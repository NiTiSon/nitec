using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class GenericValueParameterSyntax : GenericParameterSyntax
{
	public SimpleNameSyntax Name { get; }
	public TypeClauseSyntax TypeClause { get; }

	public override TextSpan Span => TextSpan.FromBounds(Name.Span, TypeClause.Span);
	public override NodeKind Kind => NodeKind.GenericValueParameter;

	internal GenericValueParameterSyntax(SyntaxTree tree, SimpleNameSyntax name, TypeClauseSyntax typeClause) : base(tree)
	{
		Name = name;
		TypeClause = typeClause;
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Name;
		yield return TypeClause;
	}
}