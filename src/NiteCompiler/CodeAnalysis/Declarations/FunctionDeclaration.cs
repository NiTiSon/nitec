using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Declarations;

internal sealed class FunctionDeclaration : Declaration
{
	public string Name { get; }
	public DeclarationModifiers Modifiers { get; }
	public FunctionDeclarationSyntax Syntax { get; }

	public FunctionDeclaration(string name, FunctionDeclarationSyntax syntax, DeclarationModifiers modifiers)
	{
		Name = name;
		Syntax = syntax;
		Modifiers = modifiers;
	}
}