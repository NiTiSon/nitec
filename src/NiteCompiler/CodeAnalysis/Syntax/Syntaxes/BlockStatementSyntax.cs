using System.Collections.Generic;
using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class BlockStatementSyntax : StatementSyntax
{
	public Token OpenBrace { get; }
	public SyntaxList<StatementSyntax> Statements { get; }
	public Token CloseBrace { get; }

	public override NodeKind Kind => NodeKind.BlockStatement;
	public override TextSpan Span => TextSpan.FromBounds(OpenBrace.Span, CloseBrace.Span);

	public BlockStatementSyntax(SyntaxTree tree, Token openBrace, SyntaxList<StatementSyntax> statements, Token closeBrace) : base(tree)
	{
		Debug.Assert(openBrace.TKind == TokenKind.OpenBrace);
		OpenBrace = openBrace;
		Statements = statements;
		Debug.Assert(closeBrace.TKind == TokenKind.CloseBrace);
		CloseBrace = closeBrace;
	}

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return OpenBrace;
		yield return Statements;
		yield return CloseBrace;
	}
}