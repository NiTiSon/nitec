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

	protected override ImmutableArray<Declaration> GetDeclarationChildren()
	{
		return ImmutableArray<Declaration>.CastUp(Children);
	}

	public new ImmutableArray<MergedItemDeclaration> Children
	{
		get
		{
			if (field.IsDefault)
			{
				ImmutableInterlocked.InterlockedInitialize(ref field, MakeChildren());
			}

			return field;
		}
	}

	private ImmutableArray<MergedItemDeclaration> MakeChildren()
	{
		List<SingleModuleDeclaration>? modules = null;
		List<SingleTypeDeclaration>? types = null;

		foreach (var decl in Declarations)
		{
			foreach (var child in decl.Children)
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

			foreach (var (_, namespaceGroup) in moduleGroups)
			{
				children.Add(MergedModuleDeclaration.Create([..namespaceGroup]));
			}
		}

		if (types != null)
		{
			throw new NotImplementedException("Types not implement yet.");
		}

		return children.ToImmutable();
	}
}