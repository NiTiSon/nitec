using System;
using System.Threading;

namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class Symbol
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

	public abstract void Accept(SymbolVisitor visitor);
	public abstract TResult? Accept<TResult>(SymbolVisitor<TResult> visitor);
	public abstract TResult? Accept<TResult, TArgument>(SymbolVisitor<TResult, TArgument> visitor, TArgument arg);

	public virtual bool IsExtern => false;

	internal virtual void ForceComplete(Predicate<Symbol>? filter, CancellationToken cancellationToken = default) {}
	internal virtual bool HasComplete(CompletionPart part) => true;
}