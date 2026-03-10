using System;
using System.Diagnostics;
using System.Threading;
using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Binding.BoundTree;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Symbols.Source;
using NiteCompiler.Compilation;

namespace NiteCompiler.Compiler;

internal sealed class FunctionCompiler : SymbolVisitor<object, object>
{
	private readonly NiteCompilation _compilation;
	private readonly Predicate<Symbol>? _filter;
	private readonly CancellationToken _cancellationToken;

	private FunctionCompiler(NiteCompilation compilation, Predicate<Symbol>? filter = null, CancellationToken cancellationToken = default)
	{
		_compilation = compilation;
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

	private static BoundBlock? BindFunctionBody(FunctionSymbol function)
	{
		if (function is SourceFunctionSymbol sourceFunction)
		{
			if (function.IsExtern)
			{
				return null;
			}

			Binder bodyBinder = sourceFunction.TryGetBodyBinder();
		}

		throw new UnreachableException();
	}

	private static bool PassesFilter(Predicate<Symbol>? filter, Symbol symbol)
	{
		return filter == null || filter(symbol);
	}
}