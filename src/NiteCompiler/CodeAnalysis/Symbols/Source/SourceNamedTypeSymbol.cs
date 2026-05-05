using System;
using System.Collections.Immutable;
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

	public override ImmutableArray<Symbol> GetMembers()
	{
		// TODO: Implement
		return [];
		throw new System.NotImplementedException();
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
				case CompletionPart.MembersCompleted:
					// TODO: Members

					_state.NotePartComplete(CompletionPart.MembersCompleted);
					break;
				default:
					_state.NotePartComplete(CompletionPart.All & ~CompletionPart.TypeSymbolAll);
					break;
			}

			_state.SpinWaitComplete(incompletePart, cancellationToken);
		}
	}
}