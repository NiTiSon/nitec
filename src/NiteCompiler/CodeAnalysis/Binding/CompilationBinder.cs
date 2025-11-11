using System.Linq;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Symbols.Source;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class CompilationBinder : Binder
{
	public override DiagnosticBag Diagnostics { get; } = [];

	public CompilationBinder(Compilation compilation) : base(null, compilation)
	{
	}

	protected override Symbol? Lookup(string name, LookupOptions options = LookupOptions.Default)
	{
		if (options.HasFlag(LookupOptions.Libraries))
		{
			if (options.HasFlag(LookupOptions.SymbolsFromThisLibrary))
			{
				return Compilation.GlobalScope.ThisLibrary.Name == name ? Compilation.GlobalScope.ThisLibrary : null;
			}

			return Compilation.GlobalScope.Libraries.FirstOrDefault(t => t.Name == name);
		}

		if (options.HasFlag(LookupOptions.Modules))
		{
			if (options.HasFlag(LookupOptions.SymbolsFromThisLibrary))
			{
				Compilation.ModuleManager.TryGetSourceModuleSymbol(name, out SourceModuleSymbol? symbol);
				return symbol;
			}
			else
			{
				return Compilation.ModuleManager.GetModule(name);
			}
		}

		return null;
	}

	public void Bind()
	{
		foreach (SourceModuleSymbol module in Compilation.GlobalScope.ThisLibrary.Modules)
		{
			ModuleBinder moduleBinder = new(this, Compilation, module);
			moduleBinder.Bind();
		}
	}
}