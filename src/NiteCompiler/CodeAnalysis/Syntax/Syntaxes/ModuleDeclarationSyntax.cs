using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class ModuleDeclarationSyntax : SyntaxNode
{
	public Token ModuleKeyword { get; }
	public ModuleNameSyntax ModuleName { get; }

	public ModuleDeclarationSyntax(Token moduleKeyword,  ModuleNameSyntax name)
	{
		ModuleKeyword = moduleKeyword;
		ModuleName = name;
	}

	public override TextSpan Span => TextSpan.FromBounds(ModuleKeyword.Span, ModuleName.Span);
	public override SyntaxKind Kind => SyntaxKind.ModuleDeclaration;

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return ModuleKeyword;
		yield return ModuleName;
	}
}