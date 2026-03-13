using System.Diagnostics;
using System.Linq;

namespace NiteCompiler.CodeAnalysis.Syntax;

internal static class LookupPosition
{
	internal static bool IsInBlock(int position, BlockStatementSyntax? block)
	{
		return block != null && IsBeforeToken(position, block, block.CloseBrace);
	}

	public static bool IsInBody(int position, FunctionBodySyntax? declarationBody)
	{
		return declarationBody != null && IsBeforeToken(position, declarationBody, declarationBody.ClosingToken);
	}

	public static bool IsInBody(int position, ModuleDeclarationSyntax? moduleBody)
	{
		return moduleBody != null && IsBeforeToken(position, moduleBody.LastToken);
	}

	private static bool IsBeforeToken(int position, SyntaxNode node, Token firstExcluded)
	{
		return IsBeforeToken(position, firstExcluded) && position >= node.Span.Start;
	}

	private static bool IsBeforeToken(int position, Token firstExcluded)
	{
		return firstExcluded.TKind == TokenKind.None || position < firstExcluded.Span.Start;
	}

	public static bool IsInModuleDeclaration(int position, ModuleDeclarationSyntax? declaration)
	{
		return declaration == null || IsBeforeToken(position, declaration, declaration.LastToken);
	}

	public static bool IsInFunctionDeclaration(int position, FunctionDeclarationSyntax? declaration)
	{
		return declaration != null && IsBeforeToken(position, declaration, declaration.ClosingToken);
	}
}