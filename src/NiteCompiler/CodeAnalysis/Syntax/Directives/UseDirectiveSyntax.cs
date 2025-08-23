using System;
using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Syntax.Expressions.Names;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax.Directives;

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