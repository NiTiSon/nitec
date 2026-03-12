using System;
using System.Diagnostics;
using System.Threading;
using NiteCompiler.CodeAnalysis.Declarations;
using NiteCompiler.Compilation;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceLibrarySymbol : LibrarySymbol
{
	public override string Name { get; }
	public override Symbol? ContainingSymbol => null;

	public SourceModuleSymbol GlobalModule { get; }
	public override NiteCompilation? DeclaringCompilation { get; }

	private CompletionPart _state;
	public SourceLibrarySymbol(NiteCompilation compilation, MergedModuleDeclaration rootModule, string name)
	{
		Name = name;
		DeclaringCompilation = compilation;

		GlobalModule = new(this, rootModule, MetadataFacts.GlobalModuleInternalName);
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
					GlobalModule.ForceComplete(null, cancellationToken);

					Debug.Assert(GlobalModule.HasComplete(CompletionPart.ModuleSymbolAll));
					_state.NotePartComplete(CompletionPart.MembersCompleted);
					break;
				default:
					_state.NotePartComplete(CompletionPart.All & ~CompletionPart.LibrarySymbolAll);
					break;
			}

			_state.SpinWaitComplete(incompletePart, cancellationToken);
		}
	}
}