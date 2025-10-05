using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler;

public sealed class FieldSymbol : Symbol
{
	public FieldSymbol(string name, Symbol containingSymbol, FieldDeclarationSyntax syntax)
	{
		ContainingSymbol = containingSymbol;
		Name = name;
		Syntax = syntax;
	}

	public override Symbol ContainingSymbol { get; }
	public override string Name { get; }
	public FieldDeclarationSyntax Syntax { get; }
	public TypeSymbol Type { get; internal set; }
	public override SymbolKind Kind => SymbolKind.Field;
}