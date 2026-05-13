using System.IO;
using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.Compilation;
using NiteCompiler.Diagnostics;
using NiteCompiler.IntermediateRepresentation.ControlFlow;
using NiteCompiler.IntermediateRepresentation.Mir;
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
		BindingDiagnosticBag diagnostics, TextWriter? astWriter = null, MetadataLibraryBuilder? metadata = null)
	{
		// TODO[API infrastructure wrongity]: IntermediateCompiler shouldn't report report about code validity
		// Verification of code should be made in previous stage, and we should invoke IntermediateBuilder only
		// when try to compile intermediate code for NLib file, not when just verify code!!!
		IntermediateBuilder builder = new IntermediateBuilder(compilation, function);
		ControlFlowGraph cfg = builder.CreateControlFlowGraph(block, diagnostics);

		if (!diagnostics.Diagnostics.HasAnyErrors)
		{
			var mir = MirBuilder.Build(compilation, function, cfg);

			if (astWriter != null)
			{
				astWriter.WriteLine(function.ToDisplayString());
				foreach (var (basicBlock, ssaBlock) in mir.Blocks)
				{
					astWriter.WriteLine($"  {basicBlock.Name}: {{");
					foreach (Instruction instruction in ssaBlock.Instructions)
					{
						astWriter.Write("    ");
						instruction.Write(astWriter);
						astWriter.WriteLine();
					}
					astWriter.WriteLine("  }");
				}
				astWriter.WriteLine();
				astWriter.Flush();
			}
		}

		return [];
	}

	private ControlFlowGraph CreateControlFlowGraph(BoundBlock body, BindingDiagnosticBag diagnostics)
	{
		return ControlFlowGraphBuilder.Build(_function, body, diagnostics);
	}
}