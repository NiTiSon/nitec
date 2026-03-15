using System;
using System.Collections.Immutable;
using System.Threading;
using NiteCompiler.CodeAnalysis.Declarations;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceTypeSymbol : TypeSymbol
{
	public MergedTypeDeclaration Declaration { get; }
	public override ContainerSymbol ContainingSymbol { get; }

	public override string Name => Declaration.Name;

	public override SpecialType SpecialType { get; }

	private CompletionPart _state;
	public SourceTypeSymbol(ContainerSymbol containingSymbol, MergedTypeDeclaration declaration)
	{
		ContainingSymbol = containingSymbol;
		Declaration = declaration;

		SpecialType = MakeSpecialType();
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