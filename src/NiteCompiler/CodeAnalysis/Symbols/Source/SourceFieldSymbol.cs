using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceFieldSymbol : FieldSymbol
{
	private TypeSymbol _type;

	public override TypeSymbol Type => _type;

	public SourceFieldSymbol(IContainerSymbol containingSymbol, string name, FieldDeclarationSyntax syntax)
		: base(containingSymbol, name, syntax) {}


	public void SetType(TypeSymbol type)
	{
		_type = type;
	}
}