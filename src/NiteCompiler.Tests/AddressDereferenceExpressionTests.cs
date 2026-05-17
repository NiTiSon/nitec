using System.Linq;
using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Symbols.Source;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Compilation;
using NiteCompiler.Diagnostics;
using NiteCompiler.IntermediateRepresentation;
using NiteCompiler.IntermediateRepresentation.ControlFlow;
using NiteCompiler.IntermediateRepresentation.Nir;

namespace NiteCompiler.Tests;

[TestFixture]
public class AddressDereferenceExpressionTests
{
	[Test]
	public void BindFunctionBody_AddressOfAndDereference_BindsReferenceTypes()
	{
		const string source = """
		public test(x: i32) -> i32 {
			let p = &x;
			let y = *p;
			return y;
		}

		module std::numerics;

		public type SInt32;
		""";

		BoundFunctionBody body = BindBody(source, "test", out BindingDiagnosticBag diagnostics);
		try
		{
			Assert.That(diagnostics.Diagnostics, Is.Empty);

			var pDeclaration = (BoundLocalVariableDeclarationStatement)body.BlockBody.Statements[0];
			var yDeclaration = (BoundLocalVariableDeclarationStatement)body.BlockBody.Statements[1];

			Assert.Multiple(() =>
			{
				Assert.That(pDeclaration.Initializer, Is.InstanceOf<BoundAddressOfExpression>());
				Assert.That(pDeclaration.Local.Type, Is.InstanceOf<BaseReferenceTypeSymbol>());
				Assert.That(((BaseReferenceTypeSymbol)pDeclaration.Local.Type).PointsTo.SpecialType, Is.EqualTo(SpecialType.StdNumericsSInt32));
				Assert.That(yDeclaration.Initializer, Is.InstanceOf<BoundDereferenceExpression>());
				Assert.That(yDeclaration.Local.Type.SpecialType, Is.EqualTo(SpecialType.StdNumericsSInt32));
			});
		}
		finally
		{
			diagnostics.Free();
		}
	}

	[Test]
	public void BindFunctionBody_AddressOfRValue_ReportsDiagnostic()
	{
		const string source = """
		public test() {
			let p = &1;
		}

		module std::numerics;

		public type SInt32;

		module std;

		public type Boolean;
		public type NeverReturn;
		public type Void;
		""";

		_ = BindBody(source, "test", out BindingDiagnosticBag diagnostics);
		try
		{
			Assert.That(diagnostics.Diagnostics.Select(diagnostic => diagnostic.Id), Does.Contain("cannot-use-as-lvalue"));
		}
		finally
		{
			diagnostics.Free();
		}
	}

	[Test]
	public void BindFunctionBody_DereferenceNonReference_ReportsDiagnostic()
	{
		const string source = """
		public test(x: i32) {
			let y = *x;
		}

		module std::numerics;

		public type SInt32;

		module std;

		public type Void;
		""";

		_ = BindBody(source, "test", out BindingDiagnosticBag diagnostics);
		try
		{
			Assert.That(diagnostics.Diagnostics.Select(diagnostic => diagnostic.Id), Does.Contain("cannot-dereference-non-reference"));
		}
		finally
		{
			diagnostics.Free();
		}
	}

	[Test]
	public void BindFunctionBody_DereferenceMutableReference_CanBeUsedAsRValue()
	{
		const string source = """
		public test() -> i32 {
			let x = 12;
			let ref_x: &i32 = &x;
			x = *ref_x;
			return x;
		}

		module std::numerics;

		public type SInt32;

		module std;

		public type Void;
		""";

		_ = BindBody(source, "test", out BindingDiagnosticBag diagnostics);
		try
		{
			Assert.That(diagnostics.Diagnostics, Is.Empty);
		}
		finally
		{
			diagnostics.Free();
		}
	}

	[Test]
	public void BuildMir_DereferenceReferenceLocal_UsesReferenceValueAsLoadAddress()
	{
		const string source = """
		public test() -> i32 {
			let x = 12;
			let ref_x: &i32 = &x;
			x = *ref_x;
			return x;
		}

		module std::numerics;

		public type SInt32;

		module std;

		public type Void;
		""";

		NirFunction nir = BuildNir(source, "test");
		Instruction[] instructions = nir.Blocks.Values
			.SelectMany(block => block.Instructions)
			.ToArray();
		Assert.Multiple(() =>
		{
			Assert.That(instructions, Has.Some.InstanceOf<LoadInstruction>());
			Assert.That(instructions, Has.Some.InstanceOf<AddressOfInstruction>());
		});
	}

	[Test]
	public void BuildMir_AddressOfDereference_EmitsReferenceInstructions()
	{
		const string source = """
		public test(x: i32) -> i32 {
			return *&x;
		}

		module std::numerics;

		public type SInt32;
		""";

		NirFunction nir = BuildNir(source, "test");
		Instruction[] instructions = nir.Blocks.Values.SelectMany(block => block.Instructions).ToArray();

		Assert.Multiple(() =>
		{
			Assert.That(instructions, Has.Some.InstanceOf<AddressOfInstruction>());
			Assert.That(instructions, Has.Some.InstanceOf<LoadInstruction>());
		});
	}

	[Test]
	public void EmitLLVMModule_DereferenceReferenceLocal_EmitsWithoutDiagnostics()
	{
		const string source = """
		public test() -> i32 {
			let x = 12;
			let ref_x: &i32 = &x;
			x = *ref_x;
			return x;
		}

		module std::numerics;

		public type SInt32;

		module std;

		public type Void;
		""";

		NiteCompilation compilation = CreateCompilation(source);
		Assert.That(compilation.GetDiagnostics(CompilationStage.Compile, includeEarlierStages: true), Is.Empty);

		string? targetTriple = null;
		var (module, _, _) = compilation.GetLlvmModule(out DiagnosticBag? diagnostics, ref targetTriple);

		Assert.That(diagnostics?.HasAnyErrors ?? false, Is.False);
		Assert.That(module.PrintToString(), Does.Not.Contain("load i32, i32 "));
	}

	[Test]
	public void EmitLLVMModule_AddressOfDereference_EmitsWithoutDiagnostics()
	{
		const string source = """
		public test(x: i32) -> i32 {
			return *&x;
		}

		module std::numerics;

		public type SInt32;
		""";

		NiteCompilation compilation = CreateCompilation(source);
		Assert.That(compilation.GetDiagnostics(CompilationStage.Compile, includeEarlierStages: true), Is.Empty);

		string? targetTriple = null;
		var (module, _, _) = compilation.GetLlvmModule(out DiagnosticBag? diagnostics, ref targetTriple);

		Assert.That(diagnostics?.HasAnyErrors ?? false, Is.False);
		Assert.That(module.PrintToString(), Does.Contain("ret i32"));
	}

	private static BoundFunctionBody BindBody(string source, string functionName, out BindingDiagnosticBag diagnostics)
	{
		NiteCompilation compilation = CreateCompilation(source);
		Assert.That(compilation.GetDeclarationDiagnostics(), Is.Empty);

		FunctionSymbol function = GetFunction(compilation, functionName);
		SourceFunctionSymbol sourceFunction = (SourceFunctionSymbol)function;

		Binder binder = sourceFunction.TryGetBodyBinder()!;
		diagnostics = BindingDiagnosticBag.GetInstance();
		return (BoundFunctionBody)binder.BindFunctionBody(sourceFunction.Syntax, diagnostics);
	}

	private static NiteCompilation CreateCompilation(string source)
	{
		SyntaxTree tree = SyntaxTree.ParseText(source, "test.nite", NiteCompilationOptions.Default);
		return NiteCompilation.Create("test", [tree], null, NiteCompilationOptions.Default, []);
	}

	private static NirFunction BuildNir(string source, string functionName)
	{
		NiteCompilation compilation = CreateCompilation(source);
		Assert.That(compilation.GetDeclarationDiagnostics(), Is.Empty);

		FunctionSymbol function = GetFunction(compilation, functionName);
		SourceFunctionSymbol sourceFunction = (SourceFunctionSymbol)function;
		Binder binder = sourceFunction.TryGetBodyBinder()!;
		BindingDiagnosticBag diagnostics = BindingDiagnosticBag.GetInstance();
		try
		{
			BoundFunctionBody body = (BoundFunctionBody)binder.BindFunctionBody(sourceFunction.Syntax, diagnostics);
			Assert.That(diagnostics.Diagnostics, Is.Empty);

			ControlFlowGraph cfg = ControlFlowGraphBuilder.Build(function, body.BlockBody, diagnostics);
			return NirBuilder.Build(cfg, function);
		}
		finally
		{
			diagnostics.Free();
		}
	}

	private static FunctionSymbol GetFunction(NiteCompilation compilation, string functionName)
	{
		return compilation.SourceLibrary.GlobalModule
			.GetMembersUnordered()
			.OfType<FunctionSymbol>()
			.Single(f => f.Name == functionName);
	}
}
