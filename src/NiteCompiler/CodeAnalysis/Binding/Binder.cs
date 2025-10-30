using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Symbols.Source;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Binding;

internal abstract class Binder
{
	private readonly Binder? _parent;
	private readonly Compilation _compilation;

	public DiagnosticBag Diagnostics { get; } = [];

	protected Binder(Binder? parent, Compilation compilation)
	{
		_parent = parent;
		_compilation = compilation;
	}
}