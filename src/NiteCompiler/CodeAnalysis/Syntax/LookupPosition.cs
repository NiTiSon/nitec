using System.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Syntax;

internal static class LookupPosition
{
	internal static bool IsInBlock(int position, BlockStatementSyntax? block)
	{
		return block != null && IsBeforeToken(position, block, block.CloseBrace);
	}

	internal static bool IsInMethodDeclaration(int position, FunctionDeclarationSyntax functionDeclaration)
	{
		Debug.Assert(functionDeclaration != null);

		return IsBeforeToken(position, functionDeclaration, body) ||
		       IsInExpressionBody(position, functionDeclaration.GetExpressionBodySyntax(), functionDeclaration.SemicolonToken);
	}

	private static bool IsBeforeToken(int position, SyntaxNode node, Token firstExcluded)
	{
		return IsBeforeToken(position, firstExcluded) && position >= node.Span.Start;
	}

	private static bool IsBeforeToken(int position, Token firstExcluded)
	{
		return firstExcluded.TKind == TokenKind.None || position < firstExcluded.Span.Start;
	}
}