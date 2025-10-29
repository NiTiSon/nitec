using System;
using CommunityToolkit.Diagnostics;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Symbols.Source;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Binding;

internal static class Declarator
{
	public static void DeclarationPass(SyntaxTree tree, ModuleManager manager, DiagnosticBag diagnostics)
	{
		ISourceContainerSymbol currentModule = manager.GetModuleDeclaration(string.Empty);
		foreach (SyntaxNode node in tree.Root.TopLevelNodes)
		{
			DeclarationPass(node, ref currentModule, manager, diagnostics);
		}
	}

	private static void DeclarationPass(SyntaxNode node, ref ISourceContainerSymbol container, ModuleManager manager,
		DiagnosticBag diagnostics)
	{
		switch (node)
		{
			case ModuleDeclarationSyntax moduleDeclaration when container is not SourceModuleSymbol: // ALL top level declarations has Module container
				diagnostics.ReportOnlyTopLevelModuleDeclarationsAreAllowed(moduleDeclaration.ContextualizedSpan);
				break;
			case ModuleDeclarationSyntax moduleDeclaration:
				container = manager.GetModuleDeclaration(moduleDeclaration.Name.GetName());
				break;
			case FunctionDeclarationSyntax functionDeclaration:
				container.Members.Add(new SourceFunctionSymbol(container, functionDeclaration.Name.GetName(), functionDeclaration));
				break;
			case TypeDeclarationSyntax typeDeclaration:
				SourceTypeSymbol type = new(container, typeDeclaration.Name.GetName(), typeDeclaration);
				container.Members.Add(type);
				ISourceContainerSymbol typeContainer = type;
				foreach (var member in typeDeclaration.Members)
				{
					DeclarationPass(member, ref typeContainer, manager, diagnostics);
				}
				Guard.IsReferenceEqualTo(type, typeContainer); // Should not be changed by type members
				break;
			case FieldDeclarationSyntax fieldDeclaration:
				container.Members.Add(new SourceFieldSymbol(container, fieldDeclaration.Name.GetName(), fieldDeclaration));
				break;
			default:
				throw new Exception("Unreachable.");
				break;
		}
	}
}