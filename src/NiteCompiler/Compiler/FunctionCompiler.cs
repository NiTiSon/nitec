using System;
using System.Diagnostics;
using System.Threading;
using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Binding.BoundTree;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Symbols.Source;
using NiteCompiler.Compilation;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.Compiler;

internal sealed class FunctionCompiler : SymbolVisitor<object, object>
{
	private readonly NiteCompilation _compilation;
	private readonly Predicate<Symbol>? _filter;
	private readonly CancellationToken _cancellationToken;

	private FunctionCompiler(NiteCompilation compilation, Predicate<Symbol>? filter = null, CancellationToken cancellationToken = default)
	{
		_compilation = compilation;
		_filter = filter;
		_cancellationToken = cancellationToken;
	}

	public static void CompileBodies(NiteCompilation compilation)
	{
		FunctionCompiler compiler = new(compilation);

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

	public override object? VisitFunction(FunctionSymbol symbol, object arg)
	{
		if (!PassesFilter(_filter, symbol))
		{
			return null;
		}

		return BindFunctionBody(symbol);
	}

	private static BoundBlock? BindFunctionBody(FunctionSymbol function)
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
				BoundNode methodBody = bodyBinder.BindFunctionBody(sourceFunction.Syntax, []);

				if (methodBody.Kind == BoundKind.FunctionBody)
				{
					var nonConstructor = (BoundFunctionBody)methodBody;
					BoundBlock body = nonConstructor.BlockBody;
					Debug.Assert(body != null);
					return body;
				}
				else
				{
					throw new NotImplementedException();
				}
			}
		}

		throw new UnreachableException();
	}

	private static bool PassesFilter(Predicate<Symbol>? filter, Symbol symbol)
	{
		return filter == null || filter(symbol);
	}
}