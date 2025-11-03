using System.Collections.Generic;
using System.IO;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Symbols.Source;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class FunctionBinder : BinderWithUsagesAndAliases
{
	public SourceFunctionSymbol Function { get; }

	public FunctionBinder(Binder? parent, Compilation compilation, SourceFunctionSymbol function)
		: base(parent, compilation, function.Usages)
	{
		Function = function;
	}

	public override void Bind()
	{

	}
}