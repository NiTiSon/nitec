using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Declarations;

internal sealed class DeclarationTreeBuilder : SyntaxVisitor<SingleItemDeclaration>
{
	private readonly SyntaxTree _syntaxTree;

	private DeclarationTreeBuilder(SyntaxTree syntaxTree)
	{
		_syntaxTree = syntaxTree;
	}

	public static RootModuleDeclaration ForTree(SyntaxTree syntaxTree)
	{
		DeclarationTreeBuilder builder = new(syntaxTree);
		return (RootModuleDeclaration)builder.Visit(syntaxTree.Root)!;
	}

	public override RootModuleDeclaration VisitCompilationUnit(CompilationUnitSyntax node)
	{
		ImmutableArray<SingleItemDeclaration> children = VisitModuleMembers(node, node.Items);

		return new RootModuleDeclaration(node.CreateReference(), children);
	}

	public override SingleModuleDeclaration VisitModuleDeclaration(ModuleDeclarationSyntax node)
	{
		var members = VisitModuleMembers(node, SyntaxList<ItemSyntax>.CastUp(node.Members));

		ModuleNameSyntax name = node.Name;
		SyntaxNode currentNode = node;

		var parts = name.Parts;
		for (int i = parts.Count - 1; i > 0; i--)
		{
			var part = parts[i];

			var module = new SingleModuleDeclaration(
				name: part.GetName(),
				syntax: currentNode.CreateReference(),
				nameLocation: (part.Location as SourceLocation)!,
				members: members);

			members = [module];
			currentNode = part;
		}

		return new SingleModuleDeclaration(
			node.Name.Parts[0].GetName(),
			node.Name.Parts[0].CreateReference(),
			(name.Location as SourceLocation)!,
			members);
	}

	private ImmutableArray<SingleItemDeclaration> VisitModuleMembers(SyntaxNode node, SyntaxList<ItemSyntax> members)
	{
		if (members.Count == 0)
		{
			return [];
		}

		List<SingleItemDeclaration> memberBuilder = new();
		foreach (var member in members)
		{
			SingleItemDeclaration? item = Visit(member);

			memberBuilder.AddNotNull(item);
		}

		return [..memberBuilder];
	}
}