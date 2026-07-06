using System;
using System.Collections.Immutable;
using System.Threading;
using NiteCompiler.Metadata;

namespace NiteCompiler.CodeAnalysis.Symbols.Metadata;

internal sealed class MetadataFunctionSymbol : FunctionSymbol
{
	private readonly MetadataLibrarySymbol _library;
	private readonly FunctionDeclarationEntry _entry;
	private readonly string _name;
	private ImmutableArray<ParameterSymbol> _parameters;
	private CompletionPart _state;

	public override string Name => _name;
	public override Symbol ContainingSymbol
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
	public override TypeSymbol? ReturnType => null;
	public override ImmutableArray<LifetimeSymbol> Lifetimes => [];
	public override ImmutableArray<LifetimeConstraint> LifetimeConstraints => [];

	public override ImmutableArray<ParameterSymbol> Parameters
	{
		get
		{
			if (_parameters.IsDefault)
			{
				ImmutableInterlocked.InterlockedInitialize(ref _parameters, MakeParameters());
			}

			return _parameters;
		}
	}

	public MetadataFunctionSymbol(MetadataLibrarySymbol library, FunctionDeclarationEntry entry)
	{
		_library = library;
		_entry = entry;
		_name = library.GetString(entry.NameId);
	}

	private ImmutableArray<ParameterSymbol> MakeParameters()
	{
		ImmutableArray<ParameterEntry> paramEntries = _entry.Parameters;
		if (paramEntries.IsEmpty)
		{
			return [];
		}

		var builder = ImmutableArray.CreateBuilder<ParameterSymbol>(paramEntries.Length);
		foreach (ParameterEntry paramEntry in paramEntries)
		{
			TypeSymbol? type = _library.GetTypeByMetadataId(paramEntry.TypeId);
			if (type == null) continue;

			string name = _library.GetString(paramEntry.NameId);
			builder.Add(new MetadataParameterSymbol(this, builder.Count, name, type));
		}

		return builder.ToImmutable();
	}

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
				case CompletionPart.Type:
					_state.NotePartComplete(CompletionPart.Type);
					break;
				case CompletionPart.LifetimeParameters:
					_state.NotePartComplete(CompletionPart.LifetimeParameters);
					break;
				case CompletionPart.GenericParameters:
					_state.NotePartComplete(CompletionPart.GenericParameters);
					break;
				case CompletionPart.Parameters:
					_state.NotePartComplete(CompletionPart.Parameters);
					break;
				default:
					_state.NotePartComplete(CompletionPart.All & ~CompletionPart.FunctionSymbolAll);
					break;
			}

			_state.SpinWaitComplete(_state.NextIncompletePart, cancellationToken);
		}
	}

	internal override bool HasComplete(CompletionPart part) => _state.HasComplete(part);
}
