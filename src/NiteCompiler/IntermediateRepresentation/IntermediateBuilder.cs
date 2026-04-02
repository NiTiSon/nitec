using System;
using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.Compilation;
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
		MetadataLibraryBuilder metadata)
	{
		IntermediateBuilder builder = new IntermediateBuilder(compilation, function);
		ControlFlowGraph cfg = ControlFlowGraphBuilder.Build(block);

		var ssa = SsaBuilder.Build(cfg, function);

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

		return [];
	}
}