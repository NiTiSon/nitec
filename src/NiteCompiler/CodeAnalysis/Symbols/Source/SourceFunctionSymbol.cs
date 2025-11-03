using System.Collections.Generic;
using System.Linq;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceFunctionSymbol : FunctionSymbol
{
	public List<UseOrUseAsDirectiveSyntax> Usages { get; } = [];

	public SourceFunctionSymbol(IContainerSymbol containingSymbol, string name, FunctionDeclarationSyntax syntax)
		: base(containingSymbol, name, syntax)
	{
		Usages = syntax.Block.Statements.OfType<UseOrUseAsDirectiveSyntax>().ToList();
	}
}