using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Declarations;

internal class SingleTypeDeclaration : SingleDeclaration
{
	private readonly ImmutableArray<SingleDeclaration> _members;
	public override DeclarationKind Kind => DeclarationKind.Type;

	public SingleTypeDeclaration(string name, SyntaxReference syntax, SourceLocation nameLocation, ImmutableArray<SingleDeclaration> members) : base(name, syntax, nameLocation)
	{
		_members = members;
	}

	protected override ImmutableArray<SingleDeclaration> GetNamespaceOrTypeDeclarationChildren()
	{
		return _members;
	}
}