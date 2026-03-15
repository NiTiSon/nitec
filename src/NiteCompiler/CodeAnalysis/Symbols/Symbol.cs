using System;
using System.Threading;
using NiteCompiler.Compilation;
using NiteCompiler.Diagnostics;

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

	public virtual NiteCompilation? DeclaringCompilation
	{
		get
		{
			Symbol? containingSymbol = ContainingSymbol;
			while (containingSymbol != null)
			{
				NiteCompilation? declaring = containingSymbol.DeclaringCompilation;
				if (declaring != null) return declaring;

				containingSymbol = containingSymbol.ContainingSymbol;
			}

			return null;
		}
	}

	public virtual string Name => string.Empty;

	public abstract string ToDisplayString();

	public abstract void Accept(SymbolVisitor visitor);
	public abstract TResult? Accept<TResult>(SymbolVisitor<TResult> visitor);
	public abstract TResult? Accept<TResult, TArgument>(SymbolVisitor<TResult, TArgument> visitor, TArgument arg);

	public virtual bool IsExtern => false;
	public virtual bool IsStatic => false;

	internal virtual void ForceComplete(Predicate<Symbol>? filter, CancellationToken cancellationToken = default) {}
	internal static void ForceCompleteMemberConditionally(Predicate<Symbol>? filter, Symbol member, CancellationToken cancellationToken)
	{
		if (filter == null || filter(member))
		{
			cancellationToken.ThrowIfCancellationRequested();
			member.ForceComplete(filter, cancellationToken);
		}
	}
	internal virtual bool HasComplete(CompletionPart part) => true;
}