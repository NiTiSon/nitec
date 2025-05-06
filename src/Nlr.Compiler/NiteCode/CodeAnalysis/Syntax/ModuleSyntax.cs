using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public sealed class ModuleSyntax : ISyntaxNode
{
	public Token ModuleKeyword { get; }
	public ModuleNameSyntax Name { get; }

	public ModuleSyntax(Token moduleKeyword, ModuleNameSyntax name)
	{
		ModuleKeyword = moduleKeyword;
		Name = name;
	}
	
	public SyntaxKind Kind => SyntaxKind.ModuleDeclaration;
}