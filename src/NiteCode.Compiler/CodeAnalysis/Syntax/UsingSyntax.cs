using System.Collections.Generic;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public sealed class UsingSyntax(SyntaxTree tree, SyntaxToken keyword, NameSyntax usingModule, SyntaxToken semicolon) : MemberDeclarationSyntax(tree, [])
{
	public override SyntaxKind Kind => SyntaxKind.Using;

	public SyntaxToken Keyword { get; } = keyword;
	public NameSyntax Module { get; } = usingModule;
	public SyntaxToken Semicolon { get; } = semicolon;

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Keyword;
		yield return Module;
		yield return Semicolon;
	}
}
