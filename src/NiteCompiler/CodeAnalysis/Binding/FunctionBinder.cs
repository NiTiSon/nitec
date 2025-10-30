using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Symbols.Source;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class FunctionBinder : BinderWithUsagesAndAliases
{
	public SourceFunctionSymbol Function { get; }

	public FunctionBinder(Binder? parent, Compilation compilation, SourceFunctionSymbol function, IEnumerable<UseDirectiveSyntax> usages) : base(parent, compilation, usages)
	{
		Function = function;
	}
}