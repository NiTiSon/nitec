using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Symbols.Source;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Binding;

internal static class Declarator
{
	public static void DeclarationPass(SyntaxTree tree, GlobalScope scope, ModuleManager manager, DiagnosticBag diagnostics)
	{
		ISourceContainerSymbol currentModule = manager.GetModuleDeclaration(string.Empty);
		foreach (SyntaxNode node in tree.Root.TopLevelNodes)
		{
			DeclarationPass(node, ref currentModule, scope, manager, diagnostics);
		}
	}

	private static void DeclarationPass(SyntaxNode node, ref ISourceContainerSymbol container, GlobalScope scope,
		ModuleManager manager, DiagnosticBag diagnostics)
	{
		if (node is ModuleDeclarationSyntax moduleDeclaration)
		{
			if (container is not SourceModuleSymbol)
			{
				diagnostics.ReportOnlyTopLevelModuleDeclarationsAreAllowed(moduleDeclaration.ContextualizedSpan);
			}
			else
			{
				container = manager.GetModuleDeclaration(moduleDeclaration.Name.GetName());
			}
		}
		else if (node is FunctionDeclarationSyntax functionDeclaration)
		{
			container.Members.Add(new SourceFunctionSymbol(container, functionDeclaration));
		}
	}
}