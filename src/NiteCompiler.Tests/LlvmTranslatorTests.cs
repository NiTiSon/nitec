using System.Diagnostics;
using LLVMSharp.Interop;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Compilation;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.Tests;

[TestFixture]
public class LlvmTranslatorTests
{
	[Test]
	public void EmitLLVMModule_LowersReachableFunctionsIntoSingleModule()
	{
		const string source = """
		public addone(x: i32) -> i32 {
			return x + 1;
		}

		public caller(x: i32) -> i32 {
			return addone(x);
		}

		module std::numerics;

		public type SInt32;

		module std;

		public type Boolean;
		public type Void;
		""";

		NiteCompilation compilation = CreateCompilation(source);
		Assert.That(compilation.GetDiagnostics(CompilationStage.Compile, includeEarlierStages: true), Is.Empty);

		using LLVMModuleRef module = compilation.GetLlvmModule(out DiagnosticBag? llvmDiagnostics);

		Assert.That(llvmDiagnostics?.HasAnyErrors ?? false, Is.False);

		string ir = module.PrintToString();
		Assert.Multiple(() =>
		{
			Assert.That(ir, Does.Contain("define i32"));
			Assert.That(ir, Does.Contain("addone"));
			Assert.That(ir, Does.Contain("caller"));
			Assert.That(ir, Does.Contain("call i32"));
		});
	}

	private static NiteCompilation CreateCompilation(string source)
	{
		SyntaxTree tree = SyntaxTree.ParseText(source, "test.nite", NiteCompilationOptions.Default);
		return NiteCompilation.Create("test", [tree], null, NiteCompilationOptions.Default, []);
	}
}
