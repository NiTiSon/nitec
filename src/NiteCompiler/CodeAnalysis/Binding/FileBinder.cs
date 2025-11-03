using System.Collections.Generic;
using System.Net;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Symbols.Source;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class FileBinder : BinderWithUsagesAndAliases
{
	public SyntaxTree Tree { get; }

	public FileBinder(Binder? parent, Compilation compilation, SyntaxTree tree)
		: base(parent, compilation, tree.Root.TopLevelNodes.OfType<UseOrUseAsDirectiveSyntax>())
	{
		Tree = tree;
	}

	public override void Bind()
	{
		foreach (var node in Tree.Root.TopLevelNodes)
		{
			switch (node)
			{
				case ModuleDeclarationSyntax module:
					SourceModuleSymbol sourceModule = _compilation.ModuleManager.GetSourceModuleSymbol(module);
					ModuleBinder binder = new(this, _compilation, sourceModule);
					binder.Bind();
					break;
			}
		}

		ModuleBinder globalBinder = new(this, _compilation, _compilation.GlobalScope.GlobalModule);
		globalBinder.Bind();
	}

	protected override bool ShallBind(Symbol symbol)
	{
		return symbol.SyntaxTree == Tree;
	}
}