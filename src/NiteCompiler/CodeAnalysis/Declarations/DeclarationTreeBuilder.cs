using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.VisualBasic.CompilerServices;
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

	public override SingleModuleDeclaration VisitModuleDeclaration(ModuleDeclarationSyntax declaration)
	{
		var members = VisitModuleMembers(declaration, SyntaxList<ItemSyntax>.CastUp(declaration.Members));

		ModuleNameSyntax name = declaration.Name;
		SyntaxNode currentNode = declaration;

		// module x; x -> should reference whole declaration syntax
		// module x::y; x -> should reference only name x; y -> should reference whole declaration syntax
		if (name.Parts.Count == 1)
		{
			return new SingleModuleDeclaration(
				name: name.Parts[0].GetName(),
				syntax: declaration.CreateReference(),
				nameLocation: (name.Parts[0].Location as SourceLocation)!,
				members: members);
		}
		SyntaxList<SimpleNameSyntax> parts = name.Parts;
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
			declaration.Name.Parts[0].GetName(),
			syntax: declaration.Name.Parts[0].CreateReference(),
			nameLocation: (name.Location as SourceLocation)!,
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

	public override SingleTypeDeclaration VisitTypeDeclaration(TypeDeclarationSyntax declaration)
	{
		var members = VisitModuleMembers(declaration, declaration.Members);

		SimpleNameSyntax name = declaration.Name;

		return new SingleTypeDeclaration(
			declaration.Name.GetName(),
			syntax: declaration.CreateReference(),
			nameLocation: (name.Location as SourceLocation)!,
			members);
	}

	private ImmutableArray<SingleItemDeclaration> VisitModuleMembers(SyntaxNode node, SyntaxList<MemberSyntax>? members)
	{
		if (members == null || members.Count == 0)
		{
			return [];
		}

		List<SingleItemDeclaration> memberBuilder = new();
		foreach (var member in members)
		{
			SingleItemDeclaration? item = Visit(member);

			memberBuilder.AddNotNull(item);
		}

		return [];
	}
}