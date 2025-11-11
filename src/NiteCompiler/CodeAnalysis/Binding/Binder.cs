using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Binding;

internal abstract class Binder
{
	private readonly Binder? _parent;
	public virtual DiagnosticBag Diagnostics => _parent?.Diagnostics ?? throw new Exception("Binder has no parent.");

	protected Binder(Binder? parent)
	{
		_parent = parent;
	}

	public Symbol? LookupModule(string moduleName)
	{
		return Lookup(moduleName, LookupOptions.Modules);
	}

	protected abstract Symbol? Lookup(string name, LookupOptions options = LookupOptions.Default);

	protected Symbol? LookupInContaining(string name, LookupOptions options = LookupOptions.Default)
		=> _parent?.Lookup(name, options);
}