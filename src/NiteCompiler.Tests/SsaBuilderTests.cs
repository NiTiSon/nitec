using System.Linq;
using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Symbols.Source;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Compilation;
using NiteCompiler.Diagnostics;
using NiteCompiler.IntermediateRepresentation.ControlFlow;
using NiteCompiler.IntermediateRepresentation.Ssa;

namespace NiteCompiler.Tests;

[TestFixture]
public class SsaBuilderTests
{
	[Test]
	public void Build_LoopCarriedParameter_InsertsPhiAtLoopHeader()
	{
		const string source = """
		module test;

		public simple_func(x: i32) -> i32 {
			loop {
				x = x + 1;

				if (x == 50) return 1;
			}
			return x;
		}

		module std::numerics;

		public type SInt32;
		public type Float32;

		module std;

		public type Boolean;
		""";

		SsaFunction ssa = BuildSsa(source, "simple_func");

		var loopHeader = ssa.Blocks.Keys.Single(block => block.Name!.StartsWith("loop.entry"));
		var loopHeaderSsa = ssa.Blocks[loopHeader];

		Assert.That(loopHeaderSsa.Phis, Has.Count.EqualTo(1));

		var phi = loopHeaderSsa.Phis.Single();
		Assert.Multiple(() =>
		{
			Assert.That(phi.Variable.Name, Is.EqualTo("x"));
			Assert.That(phi.Inputs.Keys, Is.EquivalentTo(loopHeader.Predecessors));
		});
	}

	private static SsaFunction BuildSsa(string source, string functionName)
	{
		SyntaxTree tree = SyntaxTree.ParseText(source, "test.nite", NiteCompilationOptions.Default);
		var compilation = NiteCompilation.Create("test", [tree], null, NiteCompilationOptions.Default, []);

		ModuleSymbol module = compilation.SourceLibrary.GlobalModule.GetNestedModule("test")!;
		FunctionSymbol function = module.GetMembersUnordered().OfType<FunctionSymbol>().Single(f => f.Name == functionName);
		SourceFunctionSymbol sourceFunction = (SourceFunctionSymbol)function;

		ExecutableCodeBinder binder = sourceFunction.TryGetBodyBinder()!;
		BindingDiagnosticBag diagnostics = BindingDiagnosticBag.GetInstance();

		Assert.That(compilation.GetDiagnostics(CompilationStage.Compile, includeEarlierStages: true).Any(t => t.Severity == DiagnosticSeverity.Error), Is.False);
		try
		{
			BoundFunctionBody functionBody = (BoundFunctionBody)binder.BindFunctionBody(sourceFunction.Syntax, diagnostics);
			Assert.That(diagnostics.Diagnostics, Is.Empty);

			ControlFlowGraph cfg = ControlFlowGraphBuilder.Build(function, functionBody.BlockBody, diagnostics);
			return SsaBuilder.Build(cfg, function);
		}
		finally
		{
			diagnostics.Free();
		}
	}
}
