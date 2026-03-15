using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Declarations;

internal sealed class MergedModuleDeclaration : MergedItemDeclaration
{
	public ImmutableArray<SingleModuleDeclaration> Declarations { get; }

	public MergedModuleDeclaration(ImmutableArray<SingleModuleDeclaration> declarations)
		: base(declarations.FirstOrDefault()?.Name ?? string.Empty)
	{
		Declarations = declarations;
	}

	public override DeclarationKind Kind => DeclarationKind.Module;

	public static MergedModuleDeclaration Create(ImmutableArray<SingleModuleDeclaration> declarations)
	{
		return new(declarations);
	}

	public static MergedModuleDeclaration Create(SingleModuleDeclaration declaration)
	{
		return new([declaration]);
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
			foreach (SingleModuleDeclaration declaration in Declarations)
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

	public new ImmutableArray<MergedItemDeclaration> Members
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

	private ImmutableArray<MergedItemDeclaration> MakeMembers()
	{
		List<SingleModuleDeclaration>? modules = null;
		List<SingleTypeDeclaration>? types = null;

		foreach (var decl in Declarations)
		{
			foreach (var child in decl.Members)
			{

				if (child is SingleTypeDeclaration typeDecl)
				{
					types ??= new();

					types.Add(typeDecl);
				}
				else if (child is SingleModuleDeclaration modDecl)
				{
					modules ??= new();

					modules.Add(modDecl);
				}
			}
		}

		ImmutableArray<MergedItemDeclaration>.Builder children = ImmutableArray.CreateBuilder<MergedItemDeclaration>();

		if (modules != null)
		{
			var moduleGroups = new Dictionary<string, List<SingleModuleDeclaration>>(StringComparer.Ordinal);

			foreach (var n in modules)
			{
				var builder = moduleGroups.GetOrAdd(n.Name, static () => []);

				builder.Add(n);
			}

			foreach (var (_, moduleGroup) in moduleGroups)
			{
				children.Add(MergedModuleDeclaration.Create([..moduleGroup]));
			}
		}

		if (types != null)
		{
			// TODO Upgrade when generics: add arity
			var typeGroups = new Dictionary<string, List<SingleTypeDeclaration>>(StringComparer.Ordinal);

			foreach (var n in types)
			{
				var builder = typeGroups.GetOrAdd(n.Name, static () => []);

				builder.Add(n);
			}

			foreach (var (_, typeGroup) in typeGroups)
			{
				children.Add(MergedTypeDeclaration.Create([..typeGroup]));
			}
		}

		return children.ToImmutable();
	}
}