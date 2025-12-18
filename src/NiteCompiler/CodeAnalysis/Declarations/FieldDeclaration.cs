using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Declarations;

internal sealed class FieldDeclaration : Declaration
{
	public string Name { get; }
	public DeclarationModifiers Modifiers { get; }
	public FieldDeclarationSyntax Syntax { get; }

	public FieldDeclaration(string name, FieldDeclarationSyntax syntax, DeclarationModifiers modifiers)
	{
		Name = name;
		Syntax = syntax;
		Modifiers = modifiers;
	}
}