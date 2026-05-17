namespace NiteCompiler.CodeAnalysis.Syntax;

internal static class LookupPosition
{
	private static bool IsBeforeToken(int position, SyntaxNode node, Token firstExcluded)
	{
		return IsBeforeToken(position, firstExcluded) && position >= node.Span.Start;
	}

	private static bool IsBeforeToken(int position, Token firstExcluded)
	{
		return firstExcluded.TKind == TokenKind.None || position < firstExcluded.Span.Start;
	}

	private static bool IsBeforeOrAtToken(int position, SyntaxNode node, Token firstExcluded)
	{
		return IsBeforeOrAtToken(position, firstExcluded) && position >= node.Span.Start;
	}

	private static bool IsBeforeOrAtToken(int position, Token firstExcluded)
	{
		return firstExcluded.TKind == TokenKind.None || position <= firstExcluded.Span.Start;
	}

	internal static bool IsInBlock(int position, BlockStatementSyntax? block)
	{
		return block != null && IsBeforeToken(position, block, block.CloseBrace);
	}

	public static bool IsInBody(int position, FunctionBodySyntax? declarationBody)
	{
		if (declarationBody is EmptyFunctionBodySyntax emptyBody)
		{
			return IsBeforeOrAtToken(position, emptyBody, emptyBody.SemicolonToken);
		}

		return declarationBody != null && IsBeforeToken(position, declarationBody, declarationBody.ClosingToken);
	}

	public static bool IsInBody(int position, ModuleDeclarationSyntax? moduleBody)
	{
		return moduleBody != null && IsBeforeToken(position, moduleBody.LastToken);
	}

	public static bool IsInModuleDeclaration(int position, ModuleDeclarationSyntax? declaration)
	{
		return declaration == null || IsBeforeToken(position, declaration, declaration.LastToken);
	}

	public static bool IsInFunctionDeclaration(int position, FunctionDeclarationSyntax? declaration)
	{
		if (declaration is { Body: EmptyFunctionBodySyntax empty })
		{
			return IsBeforeOrAtToken(position, declaration, empty.SemicolonToken);
		}

		return declaration != null && IsBeforeToken(position, declaration, declaration.ClosingToken);
	}

	public static bool IsInConstructorDeclaration(int position, ConstructorDeclarationSyntax? declaration)
	{
		if (declaration is { Body: EmptyFunctionBodySyntax empty })
		{
			return IsBeforeOrAtToken(position, declaration, empty.SemicolonToken);
		}

		return declaration != null && IsBeforeToken(position, declaration, declaration.Body.ClosingToken);
	}

	public static bool IsInNamedConstructorDeclaration(int position, NamedConstructorDeclarationSyntax? declaration)
	{
		return declaration != null && IsBeforeToken(position, declaration, declaration.Body.ClosingToken);
	}

	public static bool IsInTypeDeclaration(int position, TypeDeclarationSyntax? declaration)
	{
		if (declaration == null)
			return false;

		if (declaration.Body is MembersTypeBodySyntax membersBody)
			return position >= declaration.Span.Start && position < membersBody.CloseBrace.Span.Start;

		if (declaration.Body is EmptyTypeBodySyntax emptyBody)
			return position >= declaration.Span.Start && position <= emptyBody.SemicolonToken.Span.Start;

		return position >= declaration.Span.Start;
	}

	public static bool IsInTypeBody(int position, TypeDeclarationSyntax? declaration)
	{
		if (declaration == null)
			return false;

		if (declaration.Body is MembersTypeBodySyntax membersBody)
			return IsBeforeToken(position, membersBody, membersBody.CloseBrace);

		if (declaration.Body is EmptyTypeBodySyntax emptyBody)
			return IsBeforeOrAtToken(position, emptyBody, emptyBody.SemicolonToken);

		return false;
	}
}