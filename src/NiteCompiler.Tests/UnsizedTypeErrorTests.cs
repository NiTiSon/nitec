using System.Collections.Immutable;
using System.Linq;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Symbols.Source;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Compilation;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.Tests;

[TestFixture]
public class UnsizedTypeErrorTests
{
	[Test]
	public void Compilation_UnsizedLocalVariable_ReportsDiagnostic()
	{
		const string source = """
		public unsized type UnsizedFoo;

		public test() {
			let x: UnsizedFoo;
		}

		module std::numerics;
		public type SInt32;

		module std;
		public type Boolean;
		public type NeverReturn;
		public type Void;
		""";

		NiteCompilation compilation = CreateCompilation(source);
		ImmutableArray<Diagnostic> diagnostics = compilation.GetDiagnostics(CompilationStage.Compile, includeEarlierStages: true);

		Assert.That(diagnostics.Select(d => d.Id), Does.Contain("cannot-use-unsized-type"));
	}

	[Test]
	public void Compilation_UnsizedField_ReportsDiagnostic()
	{
		const string source = """
		public unsized type UnsizedFoo;

		public type MyType {
			public x: UnsizedFoo;
		}

		module std::numerics;
		public type SInt32;

		module std;
		public type Boolean;
		public type NeverReturn;
		public type Void;
		""";

		NiteCompilation compilation = CreateCompilation(source);
		ImmutableArray<Diagnostic> diagnostics = compilation.GetDiagnostics(CompilationStage.Compile, includeEarlierStages: true);

		Assert.That(diagnostics.Select(d => d.Id), Does.Contain("cannot-use-unsized-type"));
	}

	[Test]
	public void Compilation_UnsizedParameter_ReportsDiagnostic()
	{
		const string source = """
		public unsized type UnsizedFoo;

		public test(x: UnsizedFoo) { }

		module std::numerics;
		public type SInt32;

		module std;
		public type Boolean;
		public type NeverReturn;
		public type Void;
		""";

		NiteCompilation compilation = CreateCompilation(source);
		ImmutableArray<Diagnostic> diagnostics = compilation.GetDiagnostics(CompilationStage.Compile, includeEarlierStages: true);

		Assert.That(diagnostics.Select(d => d.Id), Does.Contain("cannot-use-unsized-type"));
	}

	private static NiteCompilation CreateCompilation(string source)
	{
		SyntaxTree tree = SyntaxTree.ParseText(source, "test.nite", NiteCompilationOptions.Default);
		return NiteCompilation.Create("test", [tree], null, NiteCompilationOptions.Default, []);
	}
}
