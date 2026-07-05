using System;
using System.Collections.Immutable;
using System.Threading;
using NiteCompiler.Metadata;

namespace NiteCompiler.CodeAnalysis.Symbols.Metadata;

internal sealed class MetadataModuleSymbol : ModuleSymbol
{
	private readonly MetadataLibrarySymbol _library;
	private readonly MetadataId _moduleId;
	private readonly string _name;
	private readonly ModuleDeclarationEntry _entry;
	private ImmutableArray<Symbol> _members;
	private CompletionPart _state;

	public override string Name => _name;
	public override Symbol ContainingSymbol
	{
		get
		{
			if (_entry.ContainerId is MetadataId containerId)
			{
				if (containerId.Kind == MetadataKind.ModuleDeclaration)
				{
					return _library.GetModuleById(containerId.Value)!;
				}
			}

			return _library;
		}
	}
	public override LibrarySymbol ContainingLibrary => _library;

	public MetadataModuleSymbol(MetadataLibrarySymbol library, MetadataId moduleId, string name, ModuleDeclarationEntry entry)
	{
		_library = library;
		_moduleId = moduleId;
		_name = name;
		_entry = entry;
	}

	public override ImmutableArray<Symbol> GetMembers()
	{
		if (_members.IsDefault)
		{
			ImmutableInterlocked.InterlockedInitialize(ref _members, MakeMembers());
		}

		return _members;
	}

	public override ImmutableArray<Symbol> GetMembers(string name)
	{
		var builder = ImmutableArray.CreateBuilder<Symbol>();
		foreach (Symbol member in GetMembers())
		{
			if (member.Name == name)
			{
				builder.Add(member);
			}
		}

		return builder.ToImmutable();
	}

	public override ImmutableArray<TypeSymbol> GetTypeMembers()
	{
		var builder = ImmutableArray.CreateBuilder<TypeSymbol>();
		foreach (Symbol member in GetMembers())
		{
			if (member is TypeSymbol type)
			{
				builder.Add(type);
			}
		}

		return builder.ToImmutable();
	}

	public override ImmutableArray<TypeSymbol> GetTypeMembers(string name, int? arity)
	{
		var builder = ImmutableArray.CreateBuilder<TypeSymbol>();
		foreach (Symbol member in GetMembers())
		{
			if (member is TypeSymbol type && type.Name == name)
			{
				if (arity == null || type.Arity == arity.Value)
				{
					builder.Add(type);
				}
			}
		}

		return builder.ToImmutable();
	}

	public override ImmutableArray<Symbol> GetMembersUnordered() => GetMembers();

	private ImmutableArray<Symbol> MakeMembers()
	{
		var builder = ImmutableArray.CreateBuilder<Symbol>();

		foreach (MetadataModuleSymbol nestedModule in _library.GetNestedModulesByContainer(_moduleId))
		{
			builder.Add(nestedModule);
		}

		foreach (MetadataNamedTypeSymbol type in _library.GetTypesByContainer(_moduleId))
		{
			builder.Add(type);
		}

		foreach (MetadataFunctionSymbol function in _library.GetFunctionsByContainer(_moduleId))
		{
			builder.Add(function);
		}

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

			switch (_state.NextIncompletePart)
			{
				case CompletionPart.None:
					return;
				case CompletionPart.NameToMembersMap:
					_ = GetMembers();
					_state.NotePartComplete(CompletionPart.NameToMembersMap);
					break;
				case CompletionPart.MembersCompleted:
					foreach (Symbol member in GetMembers())
					{
						member.ForceComplete(filter, cancellationToken);
					}

					_state.NotePartComplete(CompletionPart.MembersCompleted);
					break;
				default:
					_state.NotePartComplete(CompletionPart.All & ~CompletionPart.ModuleSymbolAll);
					break;
			}

			_state.SpinWaitComplete(_state.NextIncompletePart, cancellationToken);
		}
	}

	internal override bool HasComplete(CompletionPart part) => _state.HasComplete(part);
}
