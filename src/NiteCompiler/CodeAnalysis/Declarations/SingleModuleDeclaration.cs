using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Declarations;

internal class SingleModuleDeclaration : SingleItemDeclaration
{
	private readonly ImmutableArray<SingleItemDeclaration> _children;
	public override DeclarationKind Kind => DeclarationKind.Module;

	public SingleModuleDeclaration(string name, SyntaxReference syntax, SourceLocation nameLocation, ImmutableArray<SingleItemDeclaration> children)
		: base(name, syntax, nameLocation)
	{
		_children = children;
	}

	protected override ImmutableArray<SingleItemDeclaration> GetModuleOrTypeDeclarationChildren()
	{
		return _children;
	}
}