using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceFunctionSymbol : FunctionSymbol
{
	public List<UseOrUseAsDirectiveSyntax> Usages { get; } = [];

	public SourceFunctionSymbol(IContainerSymbol containingSymbol, string name, FunctionDeclarationSyntax syntax)
		: base(containingSymbol, name, syntax)
	{}
}