using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
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
		Debug.Assert(_syntaxTree.Root == node);
		ImmutableArray<SingleItemDeclaration> children = VisitModuleMembers(node, node.Items);

		return new RootModuleDeclaration(node.CreateReference(), children);
	}

	public override SingleModuleDeclaration VisitModuleDeclaration(ModuleDeclarationSyntax declaration)
	{
		var members = VisitModuleMembers(declaration, SyntaxList<ItemSyntax>.CastUp(declaration.Members));

		NameSyntax name = declaration.Name;
		SyntaxNode currentNode = declaration;

		while (name is PathNameSyntax path)
		{
			SingleModuleDeclaration module = new(
				name: name.UnqualifiedName.GetName(),
				syntax: currentNode.CreateReference(),
				nameLocation: (SourceLocation)path.Right.Location,
				members: members,
				diagnostics: []
				);

			members = [module];

			currentNode = name = path.Left;
		}

		return new SingleModuleDeclaration(
			name: name.GetName(),
			syntax: currentNode.CreateReference(),
			nameLocation: (SourceLocation)name.Location,
			members: members,
			diagnostics: []
		);
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
			members: members,
			diagnostics: []);
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