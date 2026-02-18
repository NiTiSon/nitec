using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Declarations;

internal abstract class SingleDeclaration : Declaration
{
	private readonly SyntaxReference _syntax;
	private readonly SourceLocation _nameLocation;

	protected SingleDeclaration(string name, SyntaxReference syntax, SourceLocation nameLocation) : base(name)
	{
		_syntax = syntax;
		_nameLocation = nameLocation;
	}

	protected override ImmutableArray<Declaration> GetDeclarationChildren()
	{
		return GetNamespaceOrTypeDeclarationChildren().CastArray<Declaration>();
	}

	public new ImmutableArray<SingleDeclaration> Children => GetNamespaceOrTypeDeclarationChildren();

	protected abstract ImmutableArray<SingleDeclaration> GetNamespaceOrTypeDeclarationChildren();

}