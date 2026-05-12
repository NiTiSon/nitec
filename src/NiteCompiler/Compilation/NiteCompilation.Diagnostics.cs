using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.CodeAnalysis.Text;
using NiteCompiler.Compiler;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.Compilation;

public partial class NiteCompilation
{
	private bool FilterAndAppendDiagnostics(DiagnosticBag accumulator, DiagnosticBag incoming, CancellationToken cancellationToken)
	{
		Debug.Assert(incoming != null);
		return FilterAndAppendDiagnostics(accumulator, incoming, exclude: null, cancellationToken);
	}

	private bool FilterAndAppendDiagnostics(DiagnosticBag accumulator, IEnumerable<Diagnostic> incoming, HashSet<string>? exclude, CancellationToken cancellationToken)
	{
		bool hasError = false;

		foreach (Diagnostic d in incoming)
		{
			if (exclude?.Contains(d.Id) == true)
			{
				continue;
			}

			var filtered = Options.FilterDiagnostic(d, cancellationToken);
			if (filtered == null || filtered.IsSuppressed)
			{
				continue;
			}

			if (filtered.IsUnsuppressableError)
			{
				hasError = true;
			}

			accumulator.Add(filtered);
		}

		return !hasError;
	}

	private DiagnosticBag? _lateinitDeclarationDiagnostics;
	private bool _declarationDiagnosticsFrozen;

	internal DiagnosticBag DeclarationDiagnostics
	{
		get
		{
			Debug.Assert(!_declarationDiagnosticsFrozen);
			if (_lateinitDeclarationDiagnostics == null)
			{
				var diagnostics = new DiagnosticBag();
				Interlocked.CompareExchange(ref _lateinitDeclarationDiagnostics, diagnostics, null);
			}

			return _lateinitDeclarationDiagnostics;
		}
	}

	public ImmutableArray<Diagnostic> GetParseDiagnostics(CancellationToken cancellationToken = default)
	{
		return GetDiagnostics(CompilationStage.Parse, false, symbolFilter: null, ssaWriter: null, cancellationToken);
	}

	public ImmutableArray<Diagnostic> GetDeclarationDiagnostics(CancellationToken cancellationToken = default)
	{
		return GetDiagnostics(CompilationStage.Declare, false, symbolFilter: null, ssaWriter: null, cancellationToken);
	}

	public ImmutableArray<Diagnostic> GetFunctionBodyDiagnostics(TextWriter? ssaWriter = null, CancellationToken cancellationToken = default)
	{
		return GetDiagnostics(CompilationStage.Compile, false, symbolFilter: null, ssaWriter, cancellationToken);
	}

	internal ImmutableArray<Diagnostic> GetDiagnostics(CompilationStage stage, bool includeEarlierStages,
		Predicate<Symbol>? symbolFilter = null, TextWriter? ssaWriter = null, CancellationToken cancellationToken = default)
	{
		DiagnosticBag bag = new();
		GetDiagnostics(stage, includeEarlierStages, bag, symbolFilter, ssaWriter, cancellationToken);
		return [..bag];
	}

	private void GetDiagnostics(CompilationStage stage, bool includeEarlierStages, DiagnosticBag diagnostics,
		Predicate<Symbol>? symbolFilter = null, TextWriter? ssaWriter = null, CancellationToken cancellationToken = default)
	{
		var builder = BindingDiagnosticBag.GetInstance();
		Debug.Assert(builder.Diagnostics != null);

		GetDiagnosticsWithoutSeverityFiltering(stage, includeEarlierStages, builder, symbolFilter, ssaWriter, cancellationToken);

		FilterAndAppendDiagnostics(diagnostics, builder.Diagnostics, cancellationToken);
		builder.Free();
	}

	private void GetDiagnosticsWithoutSeverityFiltering(CompilationStage stage, bool includeEarlierStages,
		BindingDiagnosticBag builder, Predicate<Symbol>? symbolFilter = null, TextWriter? ssaWriter = null,
		CancellationToken cancellationToken = default)
	{
		Debug.Assert(builder.Diagnostics != null);

		if (stage.IsInclude(CompilationStage.Parse, includeEarlierStages))
		{
			var syntaxTrees = SyntaxTrees;

			if (Options.ConcurrentBuild)
			{
				Parallel.For(
					0,
					syntaxTrees.Length,
					(int i) =>
					{
						SyntaxTree syntaxTree = syntaxTrees[i];
						builder.AddRange(syntaxTree.GetDiagnostics(cancellationToken));
					},
					cancellationToken);
			}
			else
			{
				foreach (var syntaxTree in syntaxTrees)
				{
					cancellationToken.ThrowIfCancellationRequested();
					builder.AddRange(syntaxTree.GetDiagnostics(cancellationToken));
				}

				cancellationToken.ThrowIfCancellationRequested();
			}
		}

		if (stage.IsInclude(CompilationStage.Declare, includeEarlierStages))
		{
			// TODO: Check library

			builder.AddRange(GetSourceDeclarationDiagnostics(symbolFilter: symbolFilter, cancellationToken: cancellationToken));

			cancellationToken.ThrowIfCancellationRequested();
		}

		if (stage.IsInclude(CompilationStage.Compile, includeEarlierStages))
		{
			GetDiagnosticsForAllMethodBodies(builder, doLowering: false, ssaWriter, cancellationToken);

			cancellationToken.ThrowIfCancellationRequested();
		}
	}

	private DiagnosticBag GetSourceDeclarationDiagnostics(SyntaxTree? syntaxTree = null, TextSpan? filterSpanWithinTree = null,
		Func<IEnumerable<Diagnostic>, SyntaxTree, TextSpan?, IEnumerable<Diagnostic>>? locationFilterOpt = null,
		Predicate<Symbol>? symbolFilter = null,
		CancellationToken cancellationToken = default)
	{
		SourceLocation? location = null;
		if (syntaxTree != null)
		{
			var root = syntaxTree.Root;
			location = filterSpanWithinTree.HasValue ?
				new SourceLocation(syntaxTree, filterSpanWithinTree.Value) :
				new SourceLocation(root);
		}

		// TODO: Change ForceComplete API, then Add location to arguments
		SourceLibrary.ForceComplete(symbolFilter, cancellationToken);

		if (syntaxTree != null && locationFilterOpt != null)
		{
			_declarationDiagnosticsFrozen = true;
		}

		IEnumerable<Diagnostic> result = _lateinitDeclarationDiagnostics?.AsEnumerable() ?? [];

		if (locationFilterOpt != null)
		{
			Debug.Assert(syntaxTree != null);
			result = locationFilterOpt(result, syntaxTree, filterSpanWithinTree);
		}

		return new DiagnosticBag(result);
	}

	private void GetDiagnosticsForAllMethodBodies(BindingDiagnosticBag diagnostics, bool doLowering,
		TextWriter? ssaWriter = null, CancellationToken cancellationToken = default)
	{
		Debug.Assert(diagnostics.Diagnostics != null);
		FunctionCompiler.CompileBodies(compilation: this, diagnostics, filter: null, ssaWriter, null, cancellationToken);

		// TODO: Add documentation compiler hier
	}
}