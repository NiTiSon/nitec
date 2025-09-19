using System;
using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class NameWithExplicitModuleSyntax : NameSyntax
{
	public ModuleNameSyntax Module { get; }
	public Token ColonColon { get; }
	public NameSyntax Name { get; }

	public NameWithExplicitModuleSyntax(ModuleNameSyntax module, Token colonColon, NameSyntax name)
	{
		if (name is NameWithExplicitModuleSyntax)
		{
			throw new ArgumentException("NameWithExplicitModuleSyntax can't accept other NameWithExplicitModuleSyntax instance.", nameof(name));
		}
		Module = module;
		ColonColon = colonColon;
		Name = name;
	}

	public override TextSpan Span => TextSpan.FromBounds(Module.Span, Name.Span);
	public override SyntaxKind Kind =>  SyntaxKind.NameWithExplicitModule;

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Module;
		yield return ColonColon;
		yield return Name;
	}
}