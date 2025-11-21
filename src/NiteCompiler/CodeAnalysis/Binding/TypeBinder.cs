using System;
using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class TypeBinder : Binder
{
	public TypeSymbol Type { get; }

	public TypeBinder(Compilation compilation, Binder parent, TypeSymbol type) : base(compilation, parent)
	{
		Type = type;
	}

	protected override Symbol? LookupSymbol(string name, LookupOptions options = LookupOptions.Default)
	{
		return LookupSymbolInParent(name, options);
	}

	public override BoundNode Bind(SyntaxNode syntax)
	{
		return syntax switch
		{
			TypeDeclarationSyntax type => BindType(type),
			_ => throw new ArgumentException(null, nameof(syntax))
		};
	}

	private BoundNode BindType(TypeDeclarationSyntax syntax)
	{
		var members = ImmutableArray.CreateBuilder<BoundNode>();
		foreach (MemberSyntax member in syntax.Members)
		{
			members.Add(BindMember(member));
		}

		return new BoundType(syntax, Type, members.ToImmutable());
	}

	private BoundNode BindMember(MemberSyntax member)
	{
		throw new NotImplementedException();
	}
}