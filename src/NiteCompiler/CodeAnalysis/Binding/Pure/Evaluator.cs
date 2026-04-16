using System;
using System.Collections.Generic;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Binding.Pure;

/// <summary>
/// Evaluator is designed to resolve pure expression during compile-time.
/// </summary>
internal sealed class Evaluator
{
	private readonly List<Symbol> _pureSymbols = [];

	public static ConstantValue? Evaluate(BoundExpression expression, BindingDiagnosticBag diagnostics)
	{
		Evaluator evaluator = new();

		return evaluator.EvaluateExpression(expression, diagnostics);
	}

	private ConstantValue? EvaluateExpression(BoundExpression expression, BindingDiagnosticBag diagnostics)
	{
		// 2 + 2 is pure expression
		// func(2, 1, 8) is a pure expression; when func is declared as pure function

		throw new NotImplementedException();
	}
}