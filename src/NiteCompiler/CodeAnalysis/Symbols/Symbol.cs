using System;
using System.Threading;
using NiteCompiler.CodeAnalysis.Symbols.Source;

namespace NiteCompiler.CodeAnalysis.Symbols;

internal abstract class Symbol
{
	public abstract SymbolKind Kind { get; }
	public abstract Symbol? ContainingSymbol { get; }

	public virtual LibrarySymbol? ContainingLibrary
	{
		get
		{
			if (ContainingSymbol is LibrarySymbol lib)
			{
				return lib;
			}

			return ContainingSymbol?.ContainingLibrary;
		}
	}

	public virtual string Name => string.Empty;

	internal virtual void ForceComplete(Predicate<Symbol>? filter, CancellationToken cancellationToken = default) {}
	internal virtual bool HasComplete(CompletionPart part) => true;
}