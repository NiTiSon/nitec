using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class IdentifierNameSyntax : SimpleNameSyntax
{
	public Token Identifier { get; }
	public override NodeKind Kind => NodeKind.IdentifierName;
	public override TextSpan Span => Identifier.Span;

	public IdentifierNameSyntax(SyntaxTree tree, Token identifier, string identifierText) : base(tree, identifierText)
	{
		Identifier = identifier;
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Identifier;
	}

	public override void Accept(SyntaxVisitor visitor) => visitor.VisitIdentifierName(this);
	public override TResult? Accept<TResult>(SyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitIdentifierName(this);
}