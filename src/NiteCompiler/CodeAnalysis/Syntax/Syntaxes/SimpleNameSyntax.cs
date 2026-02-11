using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class SimpleNameSyntax : NameSyntax
{
	private readonly string _name;

	public Token IdentifierToken { get; }
	public override TextSpan Span => IdentifierToken.Span;
	public override NodeKind Kind => NodeKind.SimpleNameExpression;

	public SimpleNameSyntax(SyntaxTree tree, Token identifierToken, string identifier) : base(tree)
	{
		IdentifierToken = identifierToken;
		_name = identifier;
	}

	public override string GetName()
	{
		return _name;
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return IdentifierToken;
	}
}