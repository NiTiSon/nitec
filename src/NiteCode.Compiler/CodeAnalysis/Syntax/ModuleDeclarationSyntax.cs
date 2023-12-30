using System.Collections.Generic;

namespace NiteCode.Compiler.CodeAnalysis.Syntax;

public sealed class ModuleDeclarationSyntax(SyntaxTree tree, SyntaxToken moduleKeyword, NameSyntax name, SyntaxToken semicolon) : MemberDeclarationSyntax(tree, [])
{
	public SyntaxToken ModuleKeyword { get; } = moduleKeyword;
	public NameSyntax Name { get; } = name;
	public SyntaxToken Semicolon { get; } = semicolon;

	public override SyntaxKind Kind => SyntaxKind.Module;

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return ModuleKeyword;
		yield return Name;
		yield return Semicolon;
	}
}