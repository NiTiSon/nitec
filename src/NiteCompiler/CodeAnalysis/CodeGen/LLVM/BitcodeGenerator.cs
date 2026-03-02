using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Binding.BoundTree;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Compilation;

namespace NiteCompiler.CodeAnalysis.CodeGen.LLVM;

internal sealed partial class BitcodeGenerator
{
	private readonly FunctionSymbol _function;

	private readonly SyntaxNode? _functionSyntax;

	private readonly BoundStatement _boundBody;
	// private readonly LlvmBcBuilder _builder;

	public BitcodeGenerator(
		FunctionSymbol function,
		BoundStatement boundBody,
		OptimizationLevel optimizations)
	{
		Debug.Assert(function != null);
		Debug.Assert(boundBody != null);
	}
}