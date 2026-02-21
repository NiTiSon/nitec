using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Syntax;

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
		ImmutableArray<SingleItemDeclaration> children = VisitModuleMembers(node, node.TopLevelNodes);

		return new RootModuleDeclaration(node.CreateReference(), children);
	}

	private ImmutableArray<SingleItemDeclaration> VisitModuleMembers(SyntaxNode node, SyntaxList<TopLevelSyntax> members)
	{
		// TODO: Impl
		return [];
	}
}