using NiteCode.Compiler.CodeAnalysis.Syntax;

namespace NiteCode.Compiler.CodeAnalysis;

public static class NiteCodeExtensions
{
	/// <summary>
	/// Confirm that the token is a keyword or not.
	/// </summary>
	public static bool IsKeyword(this SyntaxToken token)
	{
		return (int)token.Kind is > SyntaxKindFlags.KeywordFlag and < (int)SyntaxKind.TypeKeyword;
	}

	/// <summary>
	/// Confirm that the token is a contextual keyword or not.
	/// </summary>
	public static bool IsContextualKeyword(this SyntaxToken token)
	{
		// There no one contextual keyword yet
		return false;
	}

	/// <summary>
	/// Confirm that the token is a reserved keyword or not.
	/// </summary>
	/// <remarks>
	/// Reserved keywords are keywords that are determined like keywords, but are not used in any language constructions.
	/// </remarks>
	public static bool IsReservedKeyword(this SyntaxToken token)
	{
		// There no one reserved keyword yet
		return false;
	}
}