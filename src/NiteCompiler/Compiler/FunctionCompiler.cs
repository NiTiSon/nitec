using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Threading;
using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Symbols.Source;
using NiteCompiler.Compilation;
using NiteCompiler.Diagnostics;
using NiteCompiler.IntermediateRepresentation;
using NiteCompiler.IntermediateRepresentation.ControlFlow;
using NiteCompiler.Metadata;

namespace NiteCompiler.Compiler;

internal sealed class FunctionCompiler : SymbolVisitor<object, object>
{
	private readonly NiteCompilation _compilation;
	private readonly MetadataLibraryBuilder? _metadataBuilder;
	private readonly BindingDiagnosticBag _diagnostics;
	private readonly TextWriter? _ssaWriter;
	private readonly Predicate<Symbol>? _filter;
	private readonly CancellationToken _cancellationToken;

	private FunctionCompiler(
		NiteCompilation compilation,
		BindingDiagnosticBag diagnostics,
		TextWriter? ssaWriter = null,
		MetadataLibraryBuilder? metadataBuilder = null,
		Predicate<Symbol>? filter = null,
		CancellationToken cancellationToken = default)
	{
		_compilation = compilation;
		_metadataBuilder = metadataBuilder;
		_diagnostics = diagnostics;
		_ssaWriter = ssaWriter;
		_filter = filter;
		_cancellationToken = cancellationToken;
	}

	public static void CompileBodies(NiteCompilation compilation, BindingDiagnosticBag diagnostics,
		Predicate<Symbol>? filter = null, TextWriter? ssaWriter = null, MetadataLibraryBuilder? metadataBuilder = null,
		CancellationToken cancellationToken = default)
	{
		FunctionCompiler compiler = new(compilation, diagnostics, ssaWriter, metadataBuilder, filter, cancellationToken);

		compiler.CompileModule(compilation.SourceLibrary.GlobalModule);
	}

	private void CompileModule(ModuleSymbol symbol)
	{
		foreach (var s in symbol.GetMembersUnordered())
		{
			_cancellationToken.ThrowIfCancellationRequested();
			s.Accept(this, null);
		}
	}

	public override object? VisitModule(ModuleSymbol symbol, object arg)
	{
		if (!PassesFilter(_filter, symbol))
		{
			return null;
		}

		_cancellationToken.ThrowIfCancellationRequested();

		CompileModule(symbol);

		return null;
	}

	public override object? VisitType(TypeSymbol symbol, object arg)
	{
		if (!PassesFilter(_filter, symbol))
			return null;

		_cancellationToken.ThrowIfCancellationRequested();

		foreach (Symbol member in symbol.GetMembers())
		{
			member.Accept(this, arg);
		}

		return null;
	}

	public override BoundBlock? VisitFunction(FunctionSymbol symbol, object arg)
	{
		if (!PassesFilter(_filter, symbol))
		{
			return null;
		}

		return BindFunctionBody(symbol, _diagnostics);
	}

	private BoundBlock? BindFunctionBody(FunctionSymbol function, BindingDiagnosticBag diagnostics)
	{
		if (function is SourceFunctionSymbol sourceFunction)
		{
			if (function.IsExtern)
			{
				return null;
			}

			Binder? bodyBinder = sourceFunction.TryGetBodyBinder();
			if (bodyBinder != null)
			{
				BoundNode functionBody = bodyBinder.BindFunctionBody(sourceFunction.Syntax, diagnostics);

				BoundBlock body;
				if (functionBody.Kind == BoundKind.FunctionBody)
				{
					var nonConstructor = (BoundFunctionBody)functionBody;
					body = nonConstructor.BlockBody;
					Debug.Assert(body != null);
				}
				else
				{
					throw new NotImplementedException();
				}

				if (!functionBody.HasErrors)
				{
					ControlFlowGraph cfg = ControlFlowGraphBuilder.Build(function, body, diagnostics);

					LifetimeChecker.Check(function, body, cfg, diagnostics);

					if (!diagnostics.Diagnostics.HasAnyErrors)
					{
						var emittedBody = GenerateBody(function, diagnostics, body);
						_metadataBuilder?.SetFunctionBody(function, emittedBody);
					}
				}

				return body;
			}
		}
		else if (function is SourceConstructorSymbol sourceConstructor)
		{
			Binder? bodyBinder = sourceConstructor.TryGetBodyBinder();
			if (bodyBinder != null)
			{
				BoundNode functionBody = bodyBinder.BindFunctionBody(sourceConstructor.Syntax, diagnostics);

				BoundBlock body;
				if (functionBody.Kind == BoundKind.FunctionBody)
				{
					var nonConstructor = (BoundFunctionBody)functionBody;
					body = nonConstructor.BlockBody;
					Debug.Assert(body != null);
				}
				else
				{
					throw new NotImplementedException();
				}

				if (!functionBody.HasErrors)
				{
					ControlFlowGraph cfg = ControlFlowGraphBuilder.Build(function, body, diagnostics);

					LifetimeChecker.Check(function, body, cfg, diagnostics);

					if (!diagnostics.Diagnostics.HasAnyErrors)
					{
						var emittedBody = GenerateBody(function, diagnostics, body);
						_metadataBuilder?.SetFunctionBody(function, emittedBody);
					}
				}

				return body;
			}
		}

		throw new UnreachableException();
	}

	private FunctionBody GenerateBody(FunctionSymbol symbol, BindingDiagnosticBag diagnostics, BoundBlock block)
	{
		byte[] ir = IntermediateBuilder.Compile(_compilation, symbol, block, diagnostics, _ssaWriter, _metadataBuilder);

		return new FunctionBody([..ir]);
	}

	private static bool PassesFilter(Predicate<Symbol>? filter, Symbol symbol)
	{
		return filter == null || filter(symbol);
	}
}