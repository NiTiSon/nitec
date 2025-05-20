using System.Collections.Generic;
using Nlr.Compiler.CodeAnalysis.Syntax;

namespace Nlr.Compiler.NiteCode.CodeAnalysis.Syntax;

public sealed class FullyQualifiedTypeNameSyntax : NameSyntax
{
	public ModuleNameSyntax ModuleName { get; }
	public NameSyntax Name { get; }

	public FullyQualifiedTypeNameSyntax(ModuleNameSyntax moduleName, NameSyntax name)
	{
		ModuleName = moduleName;
		Name = name;
	}

	public override SyntaxKind Kind => SyntaxKind.FullyQualifiedName;

	public override IEnumerable<ISyntaxNode> GetChildren()
	{
		yield return ModuleName;
		yield return Name;
	}
}