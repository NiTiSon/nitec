using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using NiteCompiler.CodeAnalysis.Declarations;
using NiteCompiler.Compilation;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceNamedTypeSymbol : NamedTypeSymbol
{
	public MergedTypeDeclaration Declaration { get; }
	public override ContainerSymbol ContainingSymbol { get; }
	public override NiteCompilation DeclaringCompilation => ContainingSymbol.DeclaringCompilation!;

	public override string Name => Declaration.Name;
	public override int LifetimeArity => Declaration.LifetimeArity;
	public override int Arity => Declaration.Arity;

	public override SpecialType SpecialType { get; }
	public override Accessibility Accessibility { get; }
	public override bool IsUnsized => (_flags & Flags.IsPartial) != 0;

	private readonly Flags _flags;
	private CompletionPart _state;
	public SourceNamedTypeSymbol(ContainerSymbol containingSymbol, MergedTypeDeclaration declaration, BindingDiagnosticBag diagnostics)
	{
		ContainingSymbol = containingSymbol;
		Declaration = declaration;

		SpecialType = MakeSpecialType();

		foreach (SingleTypeDeclaration declarationType in declaration.Declarations)
		{
			diagnostics.AddRange(declarationType.Diagnostics);
		}

		ImmutableArray<SingleTypeDeclaration> decls = declaration.Declarations;

		Accessibility effectiveAccessibility = Accessibility.None;
		bool accessibilitySet = false;
		bool hasStrongDeclaration = false;

		foreach (SingleTypeDeclaration singleDecl in decls)
		{
			_flags |= MakeFlags(singleDecl.Modifiers);

			hasStrongDeclaration |= singleDecl.Accessibility != DeclarationAccessibility.MissedByInlinedDeclaration;

			if (singleDecl.Accessibility == DeclarationAccessibility.MissedByInlinedDeclaration)
			{
				continue;
			}

			if (!accessibilitySet)
			{
				effectiveAccessibility = (Accessibility)singleDecl.Accessibility;
				accessibilitySet = true;
			}
			else if (effectiveAccessibility != (Accessibility)singleDecl.Accessibility)
			{
				diagnostics.Diagnostics.ReportInconsistentTypeAccessibility(
					singleDecl.NameLocation,
					effectiveAccessibility,
					(Accessibility)singleDecl.Accessibility);
			}
		}

		if (decls.Length > 1)
		{
			foreach (SingleTypeDeclaration singleDecl in decls)
			{
				if (!singleDecl.Modifiers.HasFlag(DeclarationModifiers.Partial))
				{
					diagnostics.Diagnostics.ReportPartialModifierRequired(singleDecl.NameLocation);
				}
			}
		}

		if (!hasStrongDeclaration)
		{
			diagnostics.Diagnostics.ReportTypeRequiresStrongDeclaration(decls[0].NameLocation);
		}

		Accessibility = effectiveAccessibility;
	}

	private SpecialType MakeSpecialType()
	{
		if (ContainingSymbol.Kind == SymbolKind.Module && ContainingSymbol.DeclaringCompilation!.LookingForSpecialTypes)
		{
			string name = ToDisplayString();

			return SpecialType.GetSpecialTypeFromFullName(name);
		}

		return SpecialType.None;
	}

	[Flags]
	private enum Flags : ushort
	{
		IsUnsized = 1 << 0,
		IsPartial = 1 << 1,

		// PERF: use flags to prevent heavy operation that would fail
	}

	private static Flags MakeFlags(DeclarationModifiers modifiers)
	{
		Flags flags = 0;
		flags |= (modifiers & DeclarationModifiers.Partial) != 0 ? Flags.IsPartial : 0;
		flags |= (modifiers & DeclarationModifiers.Unsized) != 0 ? Flags.IsUnsized : 0;

		return flags;
	}

	private ImmutableArray<Symbol> _lateinitMembers;
	public override ImmutableArray<Symbol> GetMembers()
	{
		if (_lateinitMembers.IsDefault)
		{
			ImmutableInterlocked.InterlockedInitialize(ref _lateinitMembers, MakeMembers());
		}

		return _lateinitMembers;
	}


	public override ImmutableArray<Symbol> GetMembers(string name)
	{
		var members = ArrayBuilder<Symbol>.GetInstance();
		foreach (Symbol member in GetMembers())
		{
			if (member.Name == name)
			{
				members.Add(member);
			}
		}

		return members.ToImmutableAndFree();
	}

	public override ImmutableArray<TypeSymbol> GetTypeMembers()
	{
		var types = ArrayBuilder<TypeSymbol>.GetInstance();
		foreach (Symbol member in GetMembers())
		{
			if (member is TypeSymbol type) types.Add(type);
		}

		return types.ToImmutableAndFree();
	}

	public override ImmutableArray<TypeSymbol> GetTypeMembers(string name, int? arity)
	{
		var types = ArrayBuilder<TypeSymbol>.GetInstance();
		foreach (Symbol member in GetMembers())
		{
			if (member is TypeSymbol type)
			{
				if (type.Name != name) continue;

				if (arity != null && arity.Value != type.Arity) continue;

				types.Add(type);
			}
		}

		return types.ToImmutableAndFree();
	}

	private ImmutableArray<Symbol> MakeMembers()
	{
		BindingDiagnosticBag diagnostics = BindingDiagnosticBag.GetInstance();

		ImmutableArray<Symbol>.Builder builder = ImmutableArray.CreateBuilder<Symbol>();

		foreach (MergedTypeDeclaration memberDecl in Declaration.Members)
		{
			var nestedType = new SourceNamedTypeSymbol(this, memberDecl, diagnostics);
			builder.Add(nestedType);
		}

		AddDeclarationDiagnostics(diagnostics);
		diagnostics.Free();

		return builder.ToImmutable();
	}

	internal override void ForceComplete(Predicate<Symbol>? filter, CancellationToken cancellationToken = default)
	{
		if (filter?.Invoke(this) == false)
		{
			return;
		}

		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var incompletePart = _state.NextIncompletePart;
			switch (incompletePart)
			{
				case CompletionPart.None:
					return;
				case CompletionPart.LifetimeParameters:
					// TODO[lifetime]
					_state.NotePartComplete(CompletionPart.LifetimeParameters);
					break;
				case CompletionPart.GenericParameters:
					// TODO[generics]
					_state.NotePartComplete(CompletionPart.GenericParameters);
					break;
				case CompletionPart.MembersCompleted:
				{
					ImmutableArray<Symbol> members = GetMembers();

					foreach (Symbol member in members)
					{
						member.ForceComplete(filter, cancellationToken);
					}

					_state.NotePartComplete(CompletionPart.MembersCompleted);
					break;
				}
				default:
					_state.NotePartComplete(CompletionPart.All & ~CompletionPart.TypeSymbolAll);
					break;
			}

			_state.SpinWaitComplete(incompletePart, cancellationToken);
		}
	}

	internal override bool HasComplete(CompletionPart part)
	{
		return _state.HasComplete(part);
	}
}