using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Symbols.Source;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Binding;

internal abstract class Binder
{
	private readonly Binder? _parent;
	protected readonly Compilation _compilation;

	public DiagnosticBag Diagnostics { get; } = [];

	protected Binder(Binder? parent, Compilation compilation)
	{
		_parent = parent;
		// Parent of all binder either FileBinder or ModuleBinder
		if (!IsAllowedTopmostBinderType(this) && !IsAllowedTopmostBinderType(this.GetTopMostRecursively(@this => @this._parent)))
		{
			ThrowHelper.ThrowArgumentException(nameof(parent), "Topmost parent binder must be either FileBinder or ModuleBinder.");
		}
		_compilation = compilation;
	}

	public abstract void Bind();

	[MethodImpl(MethodImplOptions.AggressiveInlining)] // If added new LibraryBinder, add it there instead of ModuleBinder, and change error message in .ctor.
	protected static bool IsAllowedTopmostBinderType(Binder instance)
	{
		return instance is ModuleBinder or FileBinder;
	}

	/// <summary>
	/// Returns <see langword="true"/> when symbol is allowed to bind by <see langword="this"/> binder.
	/// </summary>
	protected virtual bool ShallBind(Symbol symbol)
	{
		return _parent?.ShallBind(symbol) ?? false;
	}
}