using System;
using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.Compilation;
using NiteCompiler.Diagnostics;
using NiteCompiler.IntermediateRepresentation.ControlFlow;
using NiteCompiler.IntermediateRepresentation.Ssa;
using NiteCompiler.Metadata;

namespace NiteCompiler.IntermediateRepresentation;

internal sealed class IntermediateBuilder
{
	private readonly NiteCompilation _compilation;
	private readonly FunctionSymbol _function;

	private IntermediateBuilder(NiteCompilation compilation, FunctionSymbol function)
	{
		_compilation = compilation;
		_function = function;
	}

	public static byte[] Compile(NiteCompilation compilation, FunctionSymbol function, BoundBlock block,
		BindingDiagnosticBag diagnostics, MetadataLibraryBuilder? metadata)
	{
		// TODO[API infrastructure wrongity]: IntermediateCompiler shouldn't report report about code validity
		// Verification of code should be made in previous stage, and we should invoke IntermediateBuilder only
		// when try to compile intermediate code for NLib file, not when just verify code!!!
		IntermediateBuilder builder = new IntermediateBuilder(compilation, function);
		ControlFlowGraph cfg = builder.CreateControlFlowGraph(block, diagnostics);

		if (!diagnostics.Diagnostics.HasAnyErrors && metadata != null)
		{
			var ssa = SsaBuilder.Build(cfg, function);

			Console.WriteLine(function.ToDisplayString());
			foreach (var (basicBlock, ssaBlock) in ssa.Blocks)
			{
				Console.WriteLine($"{basicBlock.Name}:");
				foreach (SsaPhi phi in ssaBlock.Phis)
				{
					Console.Write("  ");
					phi.Write(Console.Out);
					Console.WriteLine();
				}
				foreach (Instruction instruction in ssaBlock.Instructions)
				{
					Console.Write("  ");
					instruction.Write(Console.Out);
					Console.WriteLine();
				}
			}
		}

		return [];
	}

	private ControlFlowGraph CreateControlFlowGraph(BoundBlock body, BindingDiagnosticBag diagnostics)
	{
		return ControlFlowGraphBuilder.Build(_function, body, diagnostics);
	}
}