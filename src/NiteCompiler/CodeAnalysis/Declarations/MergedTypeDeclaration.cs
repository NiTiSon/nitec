using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Declarations;

internal sealed class MergedTypeDeclaration : MergedItemDeclaration
{
	public ImmutableArray<SingleTypeDeclaration> Declarations { get; }
	public int LifetimeArity => Declarations[0].LifetimeArity;
	public int Arity => Declarations[0].Arity;

	public MergedTypeDeclaration(ImmutableArray<SingleTypeDeclaration> declarations)
		: base(declarations.FirstOrDefault()?.Name ?? string.Empty)
	{
		Debug.Assert(declarations.Length > 0);
		Declarations = declarations;
	}

	public override DeclarationKind Kind => DeclarationKind.Type;

	public static MergedTypeDeclaration Create(ImmutableArray<SingleTypeDeclaration> declarations)
	{
		return new(declarations);
	}

	public static MergedTypeDeclaration Create(SingleTypeDeclaration declaration)
	{
		return new([declaration]);
	}

	public ImmutableArray<SyntaxReference> SyntaxReferences
	{
		get
		{
			return [..Declarations.Select(r => r.SyntaxReference)];
		}
	}

	public ImmutableArray<SourceLocation> NameLocations
	{
		get
		{
			if (Declarations.Length == 1)
			{
				return [Declarations[0].NameLocation];
			}

			var builder = ImmutableArray.CreateBuilder<SourceLocation>();
			foreach (SingleTypeDeclaration declaration in Declarations)
			{
				SourceLocation loc = declaration.NameLocation;

				builder.Add(loc);
			}
			return builder.ToImmutable();
		}
	}

	protected override ImmutableArray<Declaration> GetDeclarationMembers()
	{
		return ImmutableArray<Declaration>.CastUp(Members);
	}

	public new ImmutableArray<MergedTypeDeclaration> Members
	{
		get
		{
			if (field.IsDefault)
			{
				ImmutableInterlocked.InterlockedInitialize(ref field, MakeMembers());
			}

			return field;
		}
	}

	private ImmutableArray<MergedTypeDeclaration> MakeMembers()
	{
		List<SingleTypeDeclaration>? types = null;

		foreach (SingleTypeDeclaration decl in Declarations)
		{
			foreach (SingleItemDeclaration child in decl.Members)
			{
				if (child is SingleTypeDeclaration typeDecl)
				{
					types ??= new();
					types.Add(typeDecl);
				}
			}
		}

		if (types == null)
		{
			return [];
		}

		var typeGroups = new Dictionary<(string, int), List<SingleTypeDeclaration>>();

		foreach (SingleTypeDeclaration n in types)
		{
			List<SingleTypeDeclaration> builder = typeGroups.GetOrAdd((n.Name, n.Arity), static () => []);
			builder.Add(n);
		}

		ImmutableArray<MergedTypeDeclaration>.Builder children = ImmutableArray.CreateBuilder<MergedTypeDeclaration>();

		foreach (List<SingleTypeDeclaration> typeGroup in typeGroups.Values)
		{
			children.Add(MergedTypeDeclaration.Create([..typeGroup]));
		}

		return children.ToImmutable();
	}
}