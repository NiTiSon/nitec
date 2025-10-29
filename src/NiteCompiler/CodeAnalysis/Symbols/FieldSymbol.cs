using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class FieldSymbol : IMemberSymbol
{
	public string Name { get; }
	public FieldDeclarationSyntax? Syntax { get; }
	public IContainerSymbol ContainingSymbol { get; }
	public abstract TypeSymbol Type { get; }

	private protected FieldSymbol(IContainerSymbol containingSymbol, string name, FieldDeclarationSyntax? syntax = null)
	{
		ContainingSymbol = containingSymbol;
		Name = name;
		Syntax = syntax;
	}

}