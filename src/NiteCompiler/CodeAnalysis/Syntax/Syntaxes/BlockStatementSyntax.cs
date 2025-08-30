using System.Collections.Generic;
using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class BlockStatementSyntax : StatementSyntax
{
	public Token LeftParen { get; }
	public ImmutableArray<StatementSyntax> Statements { get; }
	public Token RightParen { get; }

	public BlockStatementSyntax(Token leftParen, ImmutableArray<StatementSyntax> statements, Token rightParen)
	{
		LeftParen = leftParen;
		Statements = statements;
		RightParen = rightParen;
	}

	public override SyntaxKind Kind => SyntaxKind.BlockStatement;
	public override TextSpan Span => TextSpan.FromBounds(LeftParen.Span.Start, RightParen.Span.End);
	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return LeftParen;
		foreach (var statement in Statements)
			yield return statement;
		yield return RightParen;
	}
}