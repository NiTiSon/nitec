using System;
using System.Collections.Immutable;
using System.Threading;
using NiteCompiler.Metadata;

namespace NiteCompiler.CodeAnalysis.Symbols.Metadata;

internal sealed class MetadataNamedTypeSymbol : NamedTypeSymbol
{
	private readonly MetadataLibrarySymbol _library;
	private readonly MetadataId _typeId;
	private readonly TypeDeclarationEntry _entry;
	private readonly string _name;
	private CompletionPart _state;

	public override string Name => _name;
	public override ContainerSymbol ContainingSymbol
	{
		get
		{
			MetadataId containerId = _entry.ContainerId;
			if (containerId.Kind == MetadataKind.ModuleDeclaration)
			{
				return _library.GetModuleById(containerId.Value)!;
			}

			return _library.GlobalModule;
		}
	}
	public override SpecialType SpecialType => _entry.SpecialType;
	public override int LifetimeArity => 0;
	public override int Arity => 0;

	public MetadataNamedTypeSymbol(MetadataLibrarySymbol library, MetadataId typeId, TypeDeclarationEntry entry)
	{
		_library = library;
		_typeId = typeId;
		_entry = entry;
		_name = library.GetString(entry.NameId);
	}

	public override ImmutableArray<Symbol> GetMembers() => [];

	public override ImmutableArray<Symbol> GetMembers(string name) => [];

	public override ImmutableArray<TypeSymbol> GetTypeMembers() => [];

	public override ImmutableArray<TypeSymbol> GetTypeMembers(string name, int? arity) => [];

	internal override void ForceComplete(Predicate<Symbol>? filter, CancellationToken cancellationToken = default)
	{
		if (filter?.Invoke(this) == false) return;

		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();

			switch (_state.NextIncompletePart)
			{
				case CompletionPart.None:
					return;
				case CompletionPart.LifetimeParameters:
					_state.NotePartComplete(CompletionPart.LifetimeParameters);
					break;
				case CompletionPart.GenericParameters:
					_state.NotePartComplete(CompletionPart.GenericParameters);
					break;
				case CompletionPart.MembersCompleted:
					_state.NotePartComplete(CompletionPart.MembersCompleted);
					break;
				default:
					_state.NotePartComplete(CompletionPart.All & ~CompletionPart.TypeSymbolAll);
					break;
			}

			_state.SpinWaitComplete(_state.NextIncompletePart, cancellationToken);
		}
	}

	internal override bool HasComplete(CompletionPart part) => _state.HasComplete(part);
}
