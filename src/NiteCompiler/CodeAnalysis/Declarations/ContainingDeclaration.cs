namespace NiteCompiler.CodeAnalysis.Declarations;

internal abstract class ContainingDeclaration : Declaration
{
	public abstract void AddMember(Declaration declaration);
}