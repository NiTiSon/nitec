using System.Linq;
using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Symbols.Source;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Compilation;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.Tests;

[TestFixture]
public class BinderInvocationTests
{
	[Test]
	public void BindFunctionBody_SimpleInvocation_BindsCallAndInfersLocalType()
	{
		const string source = """
		public caller(x: i32) {
			let y = callee(x);
		}

		public callee(x: i32) -> bool { return true; }

		module std;

		public type Boolean;
		public type Void;

		module std::numerics;

		public type Float32;
		public type SInt32;
		""";

		SyntaxTree tree = SyntaxTree.ParseText(source, "test.nite", NiteCompilationOptions.Default);
		var compilation = NiteCompilation.Create("test", [tree], null, NiteCompilationOptions.Default, []);
		Assert.That(compilation.GetDeclarationDiagnostics(), Is.Empty);

		FunctionSymbol function = compilation.SourceLibrary.GlobalModule
			.GetMembersUnordered()
			.OfType<FunctionSymbol>()
			.Single(static f => f.Name == "caller");
		SourceFunctionSymbol sourceFunction = (SourceFunctionSymbol)function;

		Binder binder = sourceFunction.TryGetBodyBinder()!;
		BindingDiagnosticBag diagnostics = BindingDiagnosticBag.GetInstance();

		try
		{
			BoundFunctionBody functionBody = (BoundFunctionBody)binder.BindFunctionBody(sourceFunction.Syntax, diagnostics);

			Assert.That(diagnostics.Diagnostics, Is.Empty);

			var declaration = (BoundLocalVariableDeclarationStatement)functionBody.BlockBody.Statements[0];
			Assert.That(declaration.Initializer, Is.InstanceOf<BoundCall>());

			BoundCall call = (BoundCall)declaration.Initializer!;
			Assert.Multiple(() =>
			{
				Assert.That(declaration.Local.Type.SpecialType, Is.EqualTo(SpecialType.StdBoolean));
				Assert.That(call.Function.Name, Is.EqualTo("callee"));
				Assert.That(call.Arguments.Length, Is.EqualTo(1));
				Assert.That(call.Arguments[0], Is.InstanceOf<BoundCopy>());
				Assert.That(call.Type.SpecialType, Is.EqualTo(SpecialType.StdBoolean));
			});

			Assert.That(
				compilation.GetDiagnostics(CompilationStage.Compile, includeEarlierStages: true)
					.Any(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error),
				Is.False);
		}
		finally
		{
			diagnostics.Free();
		}
	}
}
