using System;
using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class ModuleDefinitionSyntax : SyntaxNode
{
	public Token UseKeyword { get; }

	public ModuleNameSyntax Name { get; }

	public ModuleDefinitionSyntax(Token useKeyword, ModuleNameSyntax name)
	{
		UseKeyword = useKeyword;
		Name = name;
	}

	public override TextSpan Span => TextSpan.FromBounds(UseKeyword.Span, Name.Span);
	public override SyntaxKind Kind =>  SyntaxKind.ModuleName;
	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return UseKeyword;
		yield return Name;
	}
}