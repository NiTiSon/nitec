using System.Collections.Generic;
using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class GenericNameSyntax : SimpleNameSyntax
{
	public Token Identifier { get; }
	public GenericParameterListSyntax Parameters { get; }

	public override NodeKind Kind => NodeKind.GenericNameExpression;
	public override TextSpan Span => TextSpan.FromBounds(Identifier.Span, Parameters.Span);

	public GenericNameSyntax(SyntaxTree tree, Token identifier, string identifierText, GenericParameterListSyntax parameters)
		: base(tree, identifierText)
	{
		Debug.Assert(parameters != null);

		Identifier = identifier;
		Parameters = parameters;
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Identifier;
		yield return Parameters;
	}

	public override void Accept(SyntaxVisitor visitor) => visitor.VisitGenericName(this);
	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitGenericName(this);
}