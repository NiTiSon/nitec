using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class SimpleNameSyntax : NameSyntax
{
	public Token IdentifierToken { get; }
	public override string ShortName { get; }
	public override TextSpan Span => IdentifierToken.Span;
	public override NodeKind Kind => NodeKind.SimpleNameExpression;

	public SimpleNameSyntax(SyntaxTree tree, Token identifierToken, string identifier) : base(tree)
	{
		IdentifierToken = identifierToken;
		ShortName = identifier;
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return IdentifierToken;
	}
}