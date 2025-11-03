using System;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Symbols.Source;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class ModuleBinder : Binder
{
	public SourceModuleSymbol Module { get; }

	public ModuleBinder(Binder? parent, Compilation compilation, SourceModuleSymbol module) : base(parent, compilation)
	{
		Module = module;
	}

	public override void Bind()
	{
		foreach (Symbol symbol in Module.Members)
		{
			switch (symbol)
			{
				case SourceFunctionSymbol functionSymbol:
					FunctionBinder functionBinder = new(this, _compilation, functionSymbol);
					break;
				default:
					Console.Error.WriteLine("Unknown symbol type: " + symbol.GetType());
					break;
			}
		}
	}
}