using System.Collections.Generic;
using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public sealed class ModuleDeclarationSyntax : ISyntaxNode
{
	public Token ModuleKeyword { get; }
	public ModuleNameSyntax Name { get; }

	public ModuleDeclarationSyntax(Token moduleKeyword, ModuleNameSyntax name)
	{
		ModuleKeyword = moduleKeyword;
		Name = name;
	}
	
	public SyntaxKind Kind => SyntaxKind.ModuleDeclaration;

	public IEnumerable<ISyntaxNode> GetChildren()
	{
		yield return ModuleKeyword;
		yield return Name;
	}
}