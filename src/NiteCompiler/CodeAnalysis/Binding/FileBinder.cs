using System.Collections.Immutable;
using System.Linq;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class FileBinder : Binder
{
	private readonly ImmutableArray<MergedModuleSymbol> _includedModules;
	public SyntaxTree Tree { get; }

	public FileBinder(CompilationBinder parent, SyntaxTree tree) : base(parent)
	{
		Tree = tree;

		var builder = ImmutableArray.CreateBuilder<MergedModuleSymbol>();
		foreach (UseOrUseAsDirectiveSyntax directives in tree.Root.TopLevelNodes.OfType<UseOrUseAsDirectiveSyntax>())
		{
			switch (directives)
			{
				case UseDirectiveSyntax directive:
					MergedModuleSymbol? mergedModuleSymbol =
						parent.Compilation.ModuleManager.GetModule(directive.ModuleName.GetName());

					if (mergedModuleSymbol != null)
					{
						builder.Add(mergedModuleSymbol);
					}
					else
					{
						Diagnostics.ReportUnresolvedSymbol(directive.ModuleName.ContextualizedSpan);
					}
					break;
				// UseAsDirectiveSyntax asDirective:
			}
		}
		_includedModules = builder.ToImmutable();
	}

	protected override Symbol? Lookup(string name, LookupOptions options = LookupOptions.Default)
	{
		var matches = _includedModules
			.SelectMany(m => m.Members)
			.Where(s => s is INamedSymbol named && named.Name == name)
			.ToImmutableArray();

		if (matches.IsEmpty)
			return LookupInContaining(name, options);

		if (matches.Length > 1)
		{
			return null;
		}

		return matches[0] as Symbol;
	}
}