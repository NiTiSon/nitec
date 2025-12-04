using System;
using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Compilation;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class CompilationUnitBinder : Binder
{
	public CompilationUnitBinder(NiteCompilation niteCompilation, Binder? parent)
		: base(niteCompilation, parent)
	{
		// TODO: Resolve usages
	}

	protected override Symbol? LookupSymbol(string name, LookupOptions options = LookupOptions.Default)
	{
		// Lookup in usages
		return LookupSymbolInParent(name, options);
	}

	public override BoundNode Bind(SyntaxNode syntax)
	{
		return syntax switch
		{
			CompilationUnitSyntax cus => BindCompilationUnitSyntax(cus),
			_ => throw new ArgumentException(null, nameof(syntax))
		};
	}

	private BoundNode BindCompilationUnitSyntax(CompilationUnitSyntax syntax)
	{
		var members = ImmutableArray.CreateBuilder<BoundNode>();

		ModuleBinder? moduleBinder = null;
		foreach (SyntaxNode node in syntax.TopLevelNodes)
		{
			// switch (node)
			// {
			// 	case UseOrUseAsDirectiveSyntax:
			// 		continue;
			// 	case ModuleDeclarationSyntax moduleDeclaration:
			// 	{
			// 		string moduleName = moduleDeclaration.Name.ToString();
			// 		var moduleSymbol = Compilation.ModuleManager.GetSourceModuleSymbol(moduleName);
			// 		moduleBinder = new(Compilation, this, moduleSymbol);
			// 		continue;
			// 	}
			// 	default:
			// 		moduleBinder ??= new ModuleBinder(Compilation, this, Compilation.ModuleManager.GetModule(string.Empty)!);
			//
			// 		members.Add(moduleBinder.Bind(node));
			// 		break;
			// }
		}

		return new BoundCompilationUnit(syntax, members.ToImmutable());
	}
}