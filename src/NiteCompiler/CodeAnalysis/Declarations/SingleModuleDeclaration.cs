using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Declarations;

internal class SingleModuleDeclaration : SingleItemDeclaration
{
	private readonly ImmutableArray<SingleItemDeclaration> _members;
	public override DeclarationKind Kind => DeclarationKind.Module;

	public SingleModuleDeclaration(string name, SyntaxReference syntax, SourceLocation nameLocation, ImmutableArray<SingleItemDeclaration> members)
		: base(name, syntax, nameLocation)
	{
		_members = members;
	}

	protected override ImmutableArray<SingleItemDeclaration> GetModuleOrTypeDeclarationMembers()
	{
		return _members;
	}
}