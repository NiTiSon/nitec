using Nlr.Compiler;
using Nlr.Compiler.CodeAnalysis.Text;
using Nlr.Compiler.NiteCode;

namespace Nlr.Tests;

public class CompilationTests
{
	[Test]
	public async Task NiteCodeCompiler_CreateCompilation_Add2()
	{
		NiteCodeCompiler compiler = new(new BuildPaths());

		NiteCodeCompilation? result = compiler.CreateCompilation(new TextSource(CodeBase.ExampleAddFunction));
		
		await Assert.That(result).IsNotNull();
	}
}