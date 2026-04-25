using System.Collections.Generic;
using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class GenericNameSyntax : SimpleNameSyntax
{
	public Token Identifier { get; }
	public GenericParameterListSyntax GenericParameterList { get; } // TODO!!: replace with GenericArgumentListSyntax

	public override NodeKind Kind => NodeKind.GenericNameExpression;
	public override TextSpan Span => TextSpan.FromBounds(Identifier.Span, GenericParameterList.Span);

	internal GenericNameSyntax(SyntaxTree tree, Token identifier, string identifierText, GenericParameterListSyntax genericParameterList)
		: base(tree, identifierText)
	{
		Debug.Assert(genericParameterList != null);

		Identifier = identifier;
		GenericParameterList = genericParameterList;
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Identifier;
		yield return GenericParameterList;
	}

	public override void Accept(SyntaxVisitor visitor) => visitor.VisitGenericName(this);
	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitGenericName(this);
}