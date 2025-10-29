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
	private readonly GlobalScope _globalScope;
	private readonly ModuleManager _moduleManager;
	private readonly ModuleSymbol[] _includedModules;
	// TODO: UseAsDirective support
	public DiagnosticBag Diagnostics { get; } = [];

	protected Binder(Binder? parent, GlobalScope globalScope, ModuleManager moduleManager,
		params ReadOnlySpan<UseDirectiveSyntax> usings)
	{
		_parent = parent;
		_globalScope = globalScope;
		_moduleManager = moduleManager;

		if (usings.Length > 0)
		{
			// TODO: Add duplicates warning
			HashSet<ModuleSymbol> usedModules = new HashSet<ModuleSymbol>(usings.Length);
			foreach (UseDirectiveSyntax syntax in usings)
			{
				ImmutableArray<ModuleSymbol> includes = _moduleManager.GetModuleDeclarationWithinAllLibraries(syntax.ModuleName.GetName(), out bool exists);

				if (!exists)
				{
					Diagnostics.ReportUnresolvedSymbol(syntax.ModuleName.ContextualizedSpan);
				}
				else
				{
					foreach (ModuleSymbol moduleSymbol in includes)
					{
						usedModules.Add(moduleSymbol);
					}
				}
			}

			_includedModules = [..usedModules];
		}
		else
		{
			_includedModules = [];
		}
	}

	public TypeSymbol ResolveType(SimpleNameSyntax name)
	{
		var typeSymbols = _includedModules
			.SelectMany(t => t.Members)
			.OfType<TypeSymbol>()
			.Where(t => t is INamedSymbol named && named.Name == name.GetName());

		return ReturnCandidateOrErrorTypeSymbol(typeSymbols, name);
	}

	public TypeSymbol ResolveType(NameWithExplicitModuleSyntax name)
	{
		var typeSymbols = _includedModules
			.Where(t => t.Name == name.Module.GetName())
			.SelectMany(t => t.Members)
			.OfType<TypeSymbol>()
			.Where(t => t is INamedSymbol named && named.Name == name.Name.GetName());

		return ReturnCandidateOrErrorTypeSymbol(typeSymbols, name);
	}

	protected TypeSymbol ReturnCandidateOrErrorTypeSymbol(IEnumerable<TypeSymbol> typeSymbols, NameSyntax name)
	{
		ImmutableArray<TypeSymbol> candidates = [..typeSymbols];

		if (candidates.Length == 1)
		{
			return candidates[0];
		}
		else if (candidates.Length == 0)
		{
			Diagnostics.ReportUnresolvedSymbol(name.ContextualizedSpan);
			return new SourceErrorTypeSymbol(ErrorSymbolReason.NotFound);
		}
		else
		{
			Diagnostics.ReportAmbiguousReference(name.ContextualizedSpan, candidates);
			return new SourceErrorTypeSymbol(ErrorSymbolReason.Ambiguity, candidates);
		}
	}
}