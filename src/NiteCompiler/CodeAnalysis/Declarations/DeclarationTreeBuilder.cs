using System.Collections.Immutable;
using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Declarations;

/// <summary>
/// This class is a declaration tree builder, it's collects module and types declarations across <see cref="SyntaxTree"/>
/// and produce <see cref="MergedModuleDeclaration"/>.
/// </summary>
/// <remarks>
/// This class only AND ONLY seek for modules and types, it's completely ignores functions, fields, etc.
/// This is intentional choice.
/// </remarks>
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
		var diagnostics = BindingDiagnosticBag.GetInstance();
		var members = VisitTypeMembers(declaration, declaration.Members);
		DeclarationAccessibility accessibility = GetAccessibility(declaration.AccessibilityToken);
		DeclarationModifiers modifiers = GetModifiers(declaration.Modifiers, diagnostics);

		NameSyntax name = declaration.Name;
		SyntaxNode currentNode = declaration;

		int pendingLifetimeArity = 0;
		int pendingArity = 0;

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

			pendingLifetimeArity = inline.GenericParameterList?.LifetimeArity ?? 0;
			pendingArity = inline.GenericParameterList?.Arity ?? 0;
		}

		while (name is InlineNameSyntax inline2)
		{
			SingleTypeDeclaration type = new(
				name: name.UnqualifiedName.GetName(),
				lifetimeArity: pendingLifetimeArity,
				arity: pendingArity,
				accessibility: accessibility,
				modifiers: modifiers,
				syntax: currentNode.CreateReference(),
				nameLocation: (name.Location as SourceLocation)!,
				members: members,
				diagnostics: []
			);

			members = [type];

			currentNode = name = inline2.Left;

			pendingLifetimeArity = inline2.GenericParameterList?.LifetimeArity ?? 0;
			pendingArity = inline2.GenericParameterList?.Arity ?? 0;
		}

		return new SingleTypeDeclaration(
			name: name.GetName(),
			lifetimeArity: pendingLifetimeArity,
			arity: pendingArity,
			accessibility: accessibility,
			modifiers: modifiers,
			syntax: currentNode.CreateReference(),
			nameLocation: (name.Location as SourceLocation)!,
			members: members,
			diagnostics: diagnostics.ToImmutableAndFree()
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

	private DeclarationModifiers GetModifiers(SyntaxList<Token> modifiers, BindingDiagnosticBag diagnostics)
	{
		DeclarationModifiers result = 0;

		foreach (Token token in modifiers)
		{
			DeclarationModifiers flag = 0;
			if (token.TKind == TokenKind.Partial)
			{
				flag |= DeclarationModifiers.Partial;
			}
			else if (token.TKind == TokenKind.Unsized)
			{
				flag |= DeclarationModifiers.Unsized;
			}
			else
			{
				Debug.Fail($"GetModifiers() contains {token.TKind}");
			}

			if (!SetFlag(ref result, flag))
			{
				diagnostics.Diagnostics.ReportDuplicateModifier(token.Location, token.TKind.ToString());
			}
		}

		return result;
	}

	/// <returns><see langword="true"/> when flag is set; otherwise <see langword="false"/>.</returns>
	private static bool SetFlag(ref DeclarationModifiers modifiers, DeclarationModifiers flag)
	{
		DeclarationModifiers previous = modifiers;

		modifiers |= flag;
		return previous != modifiers;
	}
}