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

		var memberBuilder = ArrayBuilder<SingleItemDeclaration>.GetInstance();
		foreach (var member in members)
		{
			SingleItemDeclaration? item = Visit(member);

			memberBuilder.AddNotNull(item);
		}

		return memberBuilder.ToImmutableAndFree();
	}

	public override SingleTypeDeclaration VisitTypeDeclaration(TypeDeclarationSyntax declaration)
	{
		var members = VisitTypeMembers(declaration, declaration.Members);
		DeclarationAccessibility accessibility = GetAccessibility(declaration.AccessibilityToken);
		DeclarationModifiers modifiers = GetModifiers(declaration.Modifiers);

		NameSyntax name = declaration.Name;
		SyntaxNode currentNode = declaration;

		if (name is InlineNameSyntax inline)
		{
			SingleTypeDeclaration type = new(
				name: name.UnqualifiedName.GetName(),
				lifetimeArity: declaration.GenericParameterList?.LifetimeArity ?? 0,
				arity: declaration.GenericParameterList?.Arity ?? 0,
				accessibility: accessibility,
				modifiers: modifiers,
				syntax: currentNode.CreateReference(),
				nameLocation: (name.Location as SourceLocation)!,
				members: members,
				diagnostics: []
			);

			members = [type];

			currentNode = name = inline.Left;

			accessibility = DeclarationAccessibility.MissedByInlinedDeclaration;
			modifiers = DeclarationModifiers.Partial;
		}

		// The syntax
		// [accessibility] [modifiers] type X::Y::Z;
		// will produce three types
		// The "X" with partial modifier and weak (none) accessibility
		// The "X::Y" with partial modifier and weak (none) accessibility
		// The "X::Y::Z" with [modifiers] and [accessibility] accessibility
		while (name is InlineNameSyntax inline2)
		{
			SingleTypeDeclaration type = new(
				name: name.UnqualifiedName.GetName(),
				lifetimeArity: name.UnqualifiedName.LifetimeArity,
				arity: name.UnqualifiedName.Arity,
				accessibility: accessibility,
				modifiers: modifiers,
				syntax: currentNode.CreateReference(),
				nameLocation: (name.Location as SourceLocation)!,
				members: members,
				diagnostics: []
			);

			members = [type];

			currentNode = name = inline2.Left;
		}

		return new SingleTypeDeclaration(
			name: name.GetName(),
			lifetimeArity: name.LifetimeArity,
			arity: name.Arity,
			accessibility: accessibility,
			modifiers: modifiers,
			syntax: currentNode.CreateReference(),
			nameLocation: (name.Location as SourceLocation)!,
			members: members,
			diagnostics: []
			);
	}

	private ImmutableArray<SingleItemDeclaration> VisitTypeMembers(SyntaxNode node, SyntaxList<MemberSyntax>? members)
	{
		if (members == null || members.Count == 0)
		{
			return [];
		}

		var memberBuilder = ArrayBuilder<SingleItemDeclaration>.GetInstance();
		foreach (var member in members)
		{
			SingleItemDeclaration? item = Visit(member);

			memberBuilder.AddNotNull(item);
		}

		return memberBuilder.ToImmutableAndFree();
	}

	private static DeclarationAccessibility GetAccessibility(Token token)
	{
		TokenKind kind = token.TKind;
		if (kind == TokenKind.Public)
		{
			return DeclarationAccessibility.Public;
		}

		if (kind == TokenKind.Protected)
		{
			return DeclarationAccessibility.Protected;
		}

		if (kind == TokenKind.Private)
		{
			return DeclarationAccessibility.Private;
		}

		if (kind == TokenKind.Friend)
		{
			return DeclarationAccessibility.Friend;
		}

		if (kind == TokenKind.Family)
		{
			return DeclarationAccessibility.Family;
		}

		if (kind == TokenKind.Internal)
		{
			return DeclarationAccessibility.Internal;
		}

		return DeclarationAccessibility.NotDeclaredByError;
	}

	private DeclarationModifiers GetModifiers(SyntaxList<Token> modifiers)
	{
		// TODO: implement
		return DeclarationModifiers.None;
	}
}