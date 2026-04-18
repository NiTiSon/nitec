using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Declarations;

internal sealed class SingleTypeDeclaration : SingleItemDeclaration
{
	private readonly ImmutableArray<SingleItemDeclaration> _members;
	public override DeclarationKind Kind => DeclarationKind.Type;

	public SingleTypeDeclaration(string name, SyntaxReference syntax, SourceLocation nameLocation,
		ImmutableArray<SingleItemDeclaration> members, ImmutableArray<Diagnostic> diagnostics)
		: base(name, syntax, nameLocation, diagnostics)
	{
		_members = members;
	}

	protected override ImmutableArray<SingleItemDeclaration> GetModuleOrTypeDeclarationMembers()
	{
		return _members;
	}
}