using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Symbols.Source;
using NiteCompiler.Compilation;
using NiteCompiler.Diagnostics;
using NiteCompiler.IntermediateRepresentation;
using NiteCompiler.Metadata;

namespace NiteCompiler.Compiler;

internal sealed class FunctionCompiler : SymbolVisitor<object, object>
{
	private readonly NiteCompilation _compilation;
	private readonly MetadataLibraryBuilder? _metadataBuilder;
	private readonly Predicate<Symbol>? _filter;
	private readonly CancellationToken _cancellationToken;

	private FunctionCompiler(
		NiteCompilation compilation,
		MetadataLibraryBuilder? metadataBuilder = null,
		Predicate<Symbol>? filter = null,
		CancellationToken cancellationToken = default)
	{
		_compilation = compilation;
		_metadataBuilder = metadataBuilder;
		_filter = filter;
		_cancellationToken = cancellationToken;
	}

	public static void CompileBodies(NiteCompilation compilation, MetadataLibraryBuilder? metadataBuilder)
	{
		FunctionCompiler compiler = new(compilation, metadataBuilder);

		compiler.CompileModule(compilation.SourceLibrary.GlobalModule);
	}

	private void CompileModule(ModuleSymbol symbol)
	{
		foreach (var s in symbol.GetMembersUnordered())
		{
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

	public override BoundBlock? VisitFunction(FunctionSymbol symbol, object arg)
	{
		if (!PassesFilter(_filter, symbol))
		{
			return null;
		}

		return BindFunctionBody(symbol);
	}

	private BoundBlock? BindFunctionBody(FunctionSymbol function)
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
				var diagnosticBag = BindingDiagnosticBag.GetInstance();
				BoundNode functionBody = bodyBinder.BindFunctionBody(sourceFunction.Syntax, diagnosticBag);

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
					var emittedBody = GenerateBody(function, body);

					_metadataBuilder!.SetFunctionBody(function, emittedBody);
				}

				diagnosticBag.Free();
				return body;
			}
		}

		throw new UnreachableException();
	}

	private FunctionBody GenerateBody(FunctionSymbol symbol,
		BoundBlock block)
	{
		byte[] ir = IntermediateBuilder.Compile(_compilation, symbol, block, _metadataBuilder!);

		return new FunctionBody([..ir]);
	}

	private static bool PassesFilter(Predicate<Symbol>? filter, Symbol symbol)
	{
		return filter == null || filter(symbol);
	}
}