using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Declarations;

internal sealed class MergedTypeDeclaration : MergedItemDeclaration
{
	public ImmutableArray<SingleTypeDeclaration> Declarations { get; }

	public MergedTypeDeclaration(ImmutableArray<SingleTypeDeclaration> declarations)
		: base(declarations.FirstOrDefault()?.Name ?? string.Empty)
	{
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
		return ImmutableArray<Declaration>.CastUp(Children);
	}

	public new ImmutableArray<MergedTypeDeclaration> Children
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

	private ImmutableArray<MergedTypeDeclaration> MakeChildren()
	{
		return [];
		// TODO: Currently ain't no types as members of other type
	}
}