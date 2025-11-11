using System.Linq;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Symbols.Source;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class CompilationBinder : Binder
{
	public Compilation Compilation { get; }
	public override DiagnosticBag Diagnostics { get; } = [];

	public CompilationBinder(Compilation compilation) : base(null)
	{
		Compilation = compilation;
	}

	protected override Symbol? Lookup(string name, LookupOptions options = LookupOptions.Default)
	{
		if (options.HasFlag(LookupOptions.Libraries))
		{
			if (options.HasFlag(LookupOptions.PreferSymbolsFromThisLibrary))
			{
				return Compilation.GlobalScope.ThisLibrary.Name == name ? Compilation.GlobalScope.ThisLibrary : null;
			}

			return Compilation.GlobalScope.Libraries.FirstOrDefault(t => t.Name == name);
		}

		if (options.HasFlag(LookupOptions.Modules))
		{
			if (options.HasFlag(LookupOptions.PreferSymbolsFromThisLibrary))
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
}