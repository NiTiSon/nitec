using System;
using System.Diagnostics;
using System.Threading;
using NiteCompiler.CodeAnalysis.Declarations;
using NiteCompiler.Compilation;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceLibrarySymbol : LibrarySymbol
{
	public override string Name { get; }
	public override Symbol? ContainingSymbol => null;

	public override SourceModuleSymbol GlobalModule
	{
		get
		{
			if (field == null)
			{
				var diagnostics = BindingDiagnosticBag.GetInstance();
				SourceModuleSymbol globalModule = new(
					this, this,
					DeclaringCompilation.MergedRoot,
					diagnostics);

				if (Interlocked.CompareExchange(ref field, globalModule, null) == null)
				{
					AddDeclarationDiagnostics(diagnostics);
				}

				diagnostics.Free();
			}

			return field;
		}
	}
	public override NiteCompilation DeclaringCompilation { get; }

	private CompletionPart _state;
	public SourceLibrarySymbol(NiteCompilation compilation, string name)
	{
		Name = name;
		DeclaringCompilation = compilation;
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
					GlobalModule.ForceCompleteSpecialTypes();
					GlobalModule.ForceComplete(null, cancellationToken);

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