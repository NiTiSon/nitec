using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceFunctionSymbol : FunctionSymbol
{
	internal SourceFunctionSymbol(IContainerSymbol containingSymbol, string name, FunctionDeclarationSyntax syntax)
		: base(containingSymbol, name, syntax)
	{}
}