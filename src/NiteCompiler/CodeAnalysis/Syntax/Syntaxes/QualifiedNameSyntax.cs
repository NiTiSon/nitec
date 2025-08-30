using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Text;

namespace NiteCompiler.CodeAnalysis.Syntax;

public sealed class QualifiedNameSyntax : NameSyntax
{
	public ModuleNameSyntax Module { get; }
	public Token ColonColon { get; }
	public NameSyntax Name { get; }

	public QualifiedNameSyntax(ModuleNameSyntax module, Token colonColon, NameSyntax name)
	{
		Module = module;
		ColonColon = colonColon;
		Name = name;
	}

	public override TextSpan Span => TextSpan.FromBounds(Module.Span, Name.Span);
	public override SyntaxKind Kind =>  SyntaxKind.QualifiedName;

	public override IEnumerable<SyntaxNode> GetChildren()
	{
		yield return Module;
		yield return ColonColon;
		yield return Name;
	}
}