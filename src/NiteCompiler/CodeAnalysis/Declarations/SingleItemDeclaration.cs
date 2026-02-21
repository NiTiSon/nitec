using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Declarations;

internal abstract class SingleItemDeclaration : Declaration
{
	public SyntaxReference SyntaxReference { get; }
	public SourceLocation NameLocation { get; }
	public SourceLocation Location => SyntaxReference.GetLocation();

	protected SingleItemDeclaration(string name, SyntaxReference syntax, SourceLocation nameLocation) : base(name)
	{
		SyntaxReference = syntax;
		NameLocation = nameLocation;
	}

	protected override ImmutableArray<Declaration> GetDeclarationChildren()
	{
		return ImmutableArray<Declaration>.CastUp(GetModuleOrTypeDeclarationChildren());
	}

	public new ImmutableArray<SingleItemDeclaration> Children => GetModuleOrTypeDeclarationChildren();

	protected abstract ImmutableArray<SingleItemDeclaration> GetModuleOrTypeDeclarationChildren();

}