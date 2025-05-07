using System.Collections.Generic;
using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public class UseDirectiveSyntax : ISyntaxNode
{
	public Token UseKeyword { get; }
	public ModuleNameSyntax Name { get; }

	public UseDirectiveSyntax(Token useKeyword, ModuleNameSyntax name)
	{
		UseKeyword = useKeyword;
		Name = name;
	}
	
	public SyntaxKind Kind => SyntaxKind.UseDirective;
	public IEnumerable<ISyntaxNode> GetChildren()
	{
		yield return UseKeyword;
		yield return Name;
	}
}