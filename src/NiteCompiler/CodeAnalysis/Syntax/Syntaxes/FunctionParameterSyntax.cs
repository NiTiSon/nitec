using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class FunctionParameterSyntax : SyntaxNode
{
	public FunctionParameterSyntax(SimpleNameSyntax name, TypeClauseSyntax? typeClause, ExpressionSyntax? defaultValue)
	{
		Name = name;
		TypeClause = typeClause;
		DefaultValue = defaultValue;
	}

	public SimpleNameSyntax Name { get; }
	public TypeClauseSyntax? TypeClause { get; }
	public ExpressionSyntax? DefaultValue { get; }

	public override TextSpan Span => TextSpan.FromBounds(Name.Span, DefaultValue?.Span ?? TypeClause?.Span ?? Name.Span);

	public override SyntaxKind Kind =>  SyntaxKind.Parameter;

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Name;
		if (TypeClause != null) yield return TypeClause;
		if (DefaultValue != null) yield return DefaultValue;
	}
}