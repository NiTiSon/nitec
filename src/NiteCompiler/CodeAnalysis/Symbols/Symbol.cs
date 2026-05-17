using System;
using System.Diagnostics;
using System.Threading;
using NiteCompiler.CodeAnalysis.Binding.Pure;
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

	public virtual TypeSymbol? ContainingType
	{
		get
		{
			if (ContainingSymbol is TypeSymbol type)
			{
				return type;
			}

			return ContainingSymbol?.ContainingType;
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

	/// <summary>
	/// The name property of symbol. Does not include any metadata.
	/// </summary>
	/// <remarks>
	/// Never is <see langword="null"/>: if name is not valid on symbol the empty string is returned.
	/// </remarks>
	public virtual string Name => string.Empty;
	public virtual int LifetimeArity => 0;
	public virtual int Arity => 0;
	public virtual Accessibility Accessibility => Accessibility.None;
	public virtual bool IsErrorSymbol => this is IErrorSymbol;

	public abstract string ToDisplayString(SymbolFormat format = SymbolFormat.Default);

	public abstract void Accept(SymbolVisitor visitor);
	public abstract TResult? Accept<TResult>(SymbolVisitor<TResult> visitor);
	public abstract TResult? Accept<TResult, TArgument>(SymbolVisitor<TResult, TArgument> visitor, TArgument arg);

	public virtual Pureness Pureness => Pureness.None;
	public bool IsPure => Pureness != Pureness.None;
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

	internal virtual void AddDeclarationDiagnostics(BindingDiagnosticBag diagnostics)
	{
		if (!diagnostics.IsEmpty)
		{
			NiteCompilation? compilation = DeclaringCompilation;
			Debug.Assert(compilation != null);

			compilation.DeclarationDiagnostics.AddRange(diagnostics.Diagnostics);
		}
	}
}