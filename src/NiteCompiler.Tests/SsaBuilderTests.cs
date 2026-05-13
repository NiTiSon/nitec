using System.Linq;
using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Symbols.Source;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Compilation;
using NiteCompiler.Diagnostics;
using NiteCompiler.IntermediateRepresentation.ControlFlow;
using NiteCompiler.IntermediateRepresentation.Mir;

namespace NiteCompiler.Tests;

[TestFixture]
public class SsaBuilderTests
{

	internal static FunctionMir BuildMir(string source, string functionName)
	{
		SyntaxTree tree = SyntaxTree.ParseText(source, "test.nite", NiteCompilationOptions.Default);
		var compilation = NiteCompilation.Create("test", [tree], null, NiteCompilationOptions.Default, []);

		ModuleSymbol module = compilation.SourceLibrary.GlobalModule.GetNestedModule("test")!;
		FunctionSymbol function = module.GetMembersUnordered().OfType<FunctionSymbol>().Single(f => f.Name == functionName);
		SourceFunctionSymbol sourceFunction = (SourceFunctionSymbol)function;

		Binder binder = sourceFunction.TryGetBodyBinder()!;
		BindingDiagnosticBag diagnostics = BindingDiagnosticBag.GetInstance();

		Assert.That(compilation.GetDiagnostics(CompilationStage.Compile, includeEarlierStages: true).Any(t => t.Severity == DiagnosticSeverity.Error), Is.False);
		try
		{
			BoundFunctionBody functionBody = (BoundFunctionBody)binder.BindFunctionBody(sourceFunction.Syntax, diagnostics);
			Assert.That(diagnostics.Diagnostics, Is.Empty);

			ControlFlowGraph cfg = ControlFlowGraphBuilder.Build(function, functionBody.BlockBody, diagnostics);
			return MirBuilder.Build(compilation, function, cfg);
		}
		finally
		{
			diagnostics.Free();
		}
	}
}
