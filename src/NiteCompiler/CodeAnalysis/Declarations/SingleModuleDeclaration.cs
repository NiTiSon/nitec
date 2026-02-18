using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Declarations;

internal class SingleModuleDeclaration : SingleDeclaration
{
	private readonly ImmutableArray<SingleDeclaration> _members;
	public override DeclarationKind Kind => DeclarationKind.Module;

	public SingleModuleDeclaration(string name, SyntaxReference syntax, SourceLocation nameLocation, ImmutableArray<SingleDeclaration> members)
		: base(name, syntax, nameLocation)
	{
		_members = members;
	}

	protected override ImmutableArray<SingleDeclaration> GetNamespaceOrTypeDeclarationChildren()
	{
		return _members;
	}
}