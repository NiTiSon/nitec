using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class UseDirectiveSyntax : SyntaxNode
{
	public Token UseKeyword { get; }
	public ModuleNameSyntax ModuleName { get; }

	public UseDirectiveSyntax(Token useKeyword,  ModuleNameSyntax name)
	{
		UseKeyword = useKeyword;
		ModuleName = name;
	}

	public override TextSpan Span => TextSpan.FromBounds(UseKeyword.Span, ModuleName.Span);
	public override SyntaxKind Kind => SyntaxKind.UseDirective;
	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return UseKeyword;
		yield return ModuleName;
	}
}