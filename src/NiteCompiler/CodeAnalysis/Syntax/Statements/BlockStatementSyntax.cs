using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax.Statements;

public sealed class BlockStatementSyntax : StatementSyntax
{
	public Token LeftParen { get; }
	public StatementSyntax[] Statements { get; }
	public Token RightParen { get; }

	public BlockStatementSyntax(Token leftParen, StatementSyntax[] statements, Token rightParen)
	{
		LeftParen = leftParen;
		Statements = statements;
		RightParen = rightParen;
	}

	public override SyntaxKind Kind => SyntaxKind.BlockStatement;

	public override TextSpan Span => TextSpan.FromBounds(LeftParen.Span.Start, RightParen.Span.End);
}