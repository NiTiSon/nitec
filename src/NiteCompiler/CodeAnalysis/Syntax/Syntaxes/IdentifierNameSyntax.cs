using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class IdentifierNameSyntax : SimpleNameSyntax
{
	public Token Identifier { get; }
	public override NodeKind Kind => NodeKind.IdentifierNameExpression;
	public override TextSpan Span => Identifier.Span;

	internal IdentifierNameSyntax(SyntaxTree tree, Token identifier, string identifierText) : base(tree, identifierText)
	{
		Identifier = identifier;
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Identifier;
	}
}