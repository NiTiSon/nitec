using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class GenericNameSyntax : SimpleNameSyntax
{
	public Token Identifier { get; }
	public Token OpenToken { get; }
	public SyntaxList<ExpressionSyntax> GenericArguments { get; }
	public Token CloseToken { get; }

	public override NodeKind Kind => NodeKind.GenericNameExpression;
	public override TextSpan Span => TextSpan.FromBounds(Identifier.Span, CloseToken.Span);

	internal GenericNameSyntax(SyntaxTree tree, Token identifier, string identifierText, Token openToken,
		SyntaxList<ExpressionSyntax> arguments, Token closeToken)
		: base(tree, identifierText)
	{
		Identifier = identifier;

		OpenToken = openToken;
		GenericArguments = arguments;
		CloseToken = closeToken;
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Identifier;
		yield return OpenToken;
		yield return GenericArguments;
		yield return CloseToken;
	}
}