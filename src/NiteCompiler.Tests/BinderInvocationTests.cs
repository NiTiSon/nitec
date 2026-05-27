using System.Linq;
using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Binding.Conversions;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Symbols.Source;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Compilation;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.Tests;

[TestFixture]
public class BinderInvocationTests
{
	private static NiteCompilation CreateCompilation(string source)
	{
		SyntaxTree tree = SyntaxTree.ParseText(source, "test.nite", NiteCompilationOptions.Default);
		return NiteCompilation.Create("test", [tree], null, NiteCompilationOptions.Default, []);
	}

	private static (BoundCall Call, BindingDiagnosticBag Diagnostics) BindFunctionBody(string source, string callerName)
	{
		NiteCompilation compilation = CreateCompilation(source);
		Assert.That(compilation.GetDeclarationDiagnostics(), Is.Empty);

		FunctionSymbol function = compilation.SourceLibrary.GlobalModule
			.GetMembersUnordered()
			.OfType<FunctionSymbol>()
			.Single(f => f.Name == callerName);
		SourceFunctionSymbol sourceFunction = (SourceFunctionSymbol)function;

		Binder binder = sourceFunction.TryGetBodyBinder()!;
		BindingDiagnosticBag diagnostics = BindingDiagnosticBag.GetInstance();

		BoundFunctionBody functionBody = (BoundFunctionBody)binder.BindFunctionBody(sourceFunction.Syntax, diagnostics);

		var declaration = (BoundLocalVariableDeclarationStatement)functionBody.BlockBody.Statements[0];
		Assert.That(declaration.Initializer, Is.InstanceOf<BoundCall>());

		BoundCall call = (BoundCall)declaration.Initializer!;
		return (call, diagnostics);
	}

	private static BindingDiagnosticBag BindFunctionBodyExpectingErrors(string source, string callerName)
	{
		NiteCompilation compilation = CreateCompilation(source);
		Assert.That(compilation.GetDeclarationDiagnostics(), Is.Empty);

		FunctionSymbol function = compilation.SourceLibrary.GlobalModule
			.GetMembersUnordered()
			.OfType<FunctionSymbol>()
			.Single(f => f.Name == callerName);
		SourceFunctionSymbol sourceFunction = (SourceFunctionSymbol)function;

		Binder binder = sourceFunction.TryGetBodyBinder()!;
		BindingDiagnosticBag diagnostics = BindingDiagnosticBag.GetInstance();

		binder.BindFunctionBody(sourceFunction.Syntax, diagnostics);

		return diagnostics;
	}

	private const string StdPreamble = """
		module std;

		public type Boolean;
		public type Void;

		module std::numerics;

		public type SInt8;
		public type SInt16;
		public type SInt32;
		public type SInt64;
		public type UInt8;
		public type UInt16;
		public type UInt32;
		public type UInt64;
		public type Float16;
		public type Float32;
		public type Float64;
		""";

	[Test]
	public void BindFunctionBody_SimpleInvocation_BindsCallAndInfersLocalType()
	{
		const string source = """
		public caller(x: i32) {
			let y = callee(x);
		}

		public callee(x: i32) -> bool { return true; }
		""" + StdPreamble;

		(BoundCall call, BindingDiagnosticBag diagnostics) = BindFunctionBody(source, "caller");

		try
		{
			Assert.That(diagnostics.Diagnostics, Is.Empty);
			Assert.Multiple(() =>
			{
				Assert.That(call.Function.Name, Is.EqualTo("callee"));
				Assert.That(call.Arguments.Length, Is.EqualTo(1));
				Assert.That(call.Type.SpecialType, Is.EqualTo(SpecialType.StdBoolean));
			});
		}
		finally
		{
			diagnostics.Free();
		}
	}

	[Test]
	public void OverloadResolved_ByArgumentCount_SelectsCorrectOverload()
	{
		const string source = """
		public caller() {
			let y = overloaded(42);
		}

		public overloaded() -> i32 { return 0; }
		public overloaded(x: i32) -> bool { return true; }
		public overloaded(x: i32, y: i32) -> i64 { return 0; }
		""" + StdPreamble;

		(BoundCall call, BindingDiagnosticBag diagnostics) = BindFunctionBody(source, "caller");

		try
		{
			Assert.That(diagnostics.Diagnostics, Is.Empty);
			Assert.Multiple(() =>
			{
				Assert.That(call.Function.Name, Is.EqualTo("overloaded"));
				Assert.That(call.Arguments.Length, Is.EqualTo(1));
				Assert.That(call.Type.SpecialType, Is.EqualTo(SpecialType.StdBoolean));
			});
		}
		finally
		{
			diagnostics.Free();
		}
	}

	[Test]
	public void OverloadResolved_ByImplicitNumericWidening_SelectsExactMatch()
	{
		const string source = """
		public caller() {
			let y = widen(42);
		}

		public widen(x: i64) -> bool { return true; }
		public widen(x: i32) -> i32 { return 0; }
		""" + StdPreamble;

		(BoundCall call, BindingDiagnosticBag diagnostics) = BindFunctionBody(source, "caller");

		try
		{
			Assert.That(diagnostics.Diagnostics, Is.Empty);
			Assert.Multiple(() =>
			{
				Assert.That(call.Function.Name, Is.EqualTo("widen"));
				Assert.That(call.Arguments.Length, Is.EqualTo(1));
				Assert.That(call.Type.SpecialType, Is.EqualTo(SpecialType.StdNumericsSInt32));
			});
		}
		finally
		{
			diagnostics.Free();
		}
	}

	[Test]
	public void OverloadResolved_ExactOverWidening_PrefersExactMatch()
	{
		const string source = """
		public caller(x: i32) {
			let y = foo(x);
		}

		public foo(x: i32) -> bool { return true; }
		public foo(x: i64) -> i32 { return 0; }
		""" + StdPreamble;

		(BoundCall call, BindingDiagnosticBag diagnostics) = BindFunctionBody(source, "caller");

		try
		{
			Assert.That(diagnostics.Diagnostics, Is.Empty);
			Assert.Multiple(() =>
			{
				Assert.That(call.Function.Name, Is.EqualTo("foo"));
				Assert.That(call.Type.SpecialType, Is.EqualTo(SpecialType.StdBoolean));
			});
		}
		finally
		{
			diagnostics.Free();
		}
	}

	[Test]
	public void OverloadResolved_NumericWidening_SelectsBetterOverload()
	{
		const string source = """
		public caller(x: i8) {
			let y = bar(x);
		}

		public bar(x: i16) -> bool { return true; }
		public bar(x: i32) -> i32 { return 0; }
		""" + StdPreamble;

		(BoundCall call, BindingDiagnosticBag diagnostics) = BindFunctionBody(source, "caller");

		try
		{
			Assert.That(diagnostics.Diagnostics, Is.Empty);
			Assert.Multiple(() =>
			{
				Assert.That(call.Function.Name, Is.EqualTo("bar"));
				Assert.That(call.Arguments.Length, Is.EqualTo(1));
				Assert.That(call.Type.SpecialType, Is.EqualTo(SpecialType.StdBoolean));
			});
		}
		finally
		{
			diagnostics.Free();
		}
	}

	[Test]
	public void OverloadResolutionFailure_NoMatchingOverload_ReportsError()
	{
		const string source = """
		public caller() {
			let y = noMatch(true);
		}

		public noMatch(x: i32) -> bool { return true; }
		""" + StdPreamble;

		BindingDiagnosticBag diagnostics = BindFunctionBodyExpectingErrors(source, "caller");

		try
		{
			Assert.That(diagnostics.Diagnostics, Is.Not.Empty);
			string diagnosticIds = string.Join(", ", diagnostics.Diagnostics.Select(d => d.Id));
			Assert.That(diagnosticIds, Does.Contain("cannot-implicitly-convert"));
		}
		finally
		{
			diagnostics.Free();
		}
	}

	[Test]
	public void OverloadResolutionFailure_WrongArgumentCount_ReportsError()
	{
		const string source = """
		public caller() {
			let y = wrongArity(42, 99);
		}

		public wrongArity(x: i32) -> bool { return true; }
		""" + StdPreamble;

		BindingDiagnosticBag diagnostics = BindFunctionBodyExpectingErrors(source, "caller");

		try
		{
			Assert.That(diagnostics.Diagnostics, Is.Not.Empty);
			string diagnosticIds = string.Join(", ", diagnostics.Diagnostics.Select(d => d.Id));
			Assert.That(diagnosticIds, Does.Contain("cannot-resolve-function"));
		}
		finally
		{
			diagnostics.Free();
		}
	}

	[Test]
	public void ConversionKind_Identity_ForSameType()
	{
		// Unit test the TypeConversions directly
		TypeSymbol i32 = CreateCompilation(StdPreamble).GetSpecialType(SpecialType.StdNumericsSInt32);
		ConversionKind kind = TypeConversions.ClassifyConversion(i32, i32);
		Assert.That(kind, Is.EqualTo(ConversionKind.Identity));
	}

	[Test]
	public void ConversionKind_ImplicitNumeric_Widening()
	{
		NiteCompilation comp = CreateCompilation(StdPreamble);
		TypeSymbol i32 = comp.GetSpecialType(SpecialType.StdNumericsSInt32);
		TypeSymbol i64 = comp.GetSpecialType(SpecialType.StdNumericsSInt64);

		ConversionKind kind = TypeConversions.ClassifyConversion(i32, i64);
		Assert.That(kind, Is.EqualTo(ConversionKind.ImplicitNumeric));
	}

	[Test]
	public void ConversionKind_None_ForUnrelatedTypes()
	{
		NiteCompilation comp = CreateCompilation(StdPreamble);
		TypeSymbol i32 = comp.GetSpecialType(SpecialType.StdNumericsSInt32);
		TypeSymbol boolean = comp.GetSpecialType(SpecialType.StdBoolean);

		ConversionKind kind = TypeConversions.ClassifyConversion(i32, boolean);
		Assert.That(kind, Is.EqualTo(ConversionKind.NoConversion));
	}

	[Test]
	public void ConversionKind_Narrowing_IsNone()
	{
		NiteCompilation comp = CreateCompilation(StdPreamble);
		TypeSymbol i32 = comp.GetSpecialType(SpecialType.StdNumericsSInt32);
		TypeSymbol i16 = comp.GetSpecialType(SpecialType.StdNumericsSInt16);

		ConversionKind kind = TypeConversions.ClassifyConversion(i32, i16);
		Assert.That(kind, Is.EqualTo(ConversionKind.ExplicitNumericTruncate));
	}

	[Test]
	public void OverloadResolved_FloatToFloatWidening_SelectsExactMatch()
	{
		const string source = """
		public caller(x: f32) {
			let y = floatOverload(x);
		}

		public floatOverload(x: f32) -> bool { return true; }
		public floatOverload(x: f64) -> i32 { return 0; }
		""" + StdPreamble;

		(BoundCall call, BindingDiagnosticBag diagnostics) = BindFunctionBody(source, "caller");

		try
		{
			Assert.That(diagnostics.Diagnostics, Is.Empty);
			Assert.Multiple(() =>
			{
				Assert.That(call.Function.Name, Is.EqualTo("floatOverload"));
				Assert.That(call.Type.SpecialType, Is.EqualTo(SpecialType.StdBoolean));
			});
		}
		finally
		{
			diagnostics.Free();
		}
	}

	[Test]
	public void OverloadResolved_IntegerToFloatWidening()
	{
		const string source = """
		public caller(x: i32) {
			let y = intToFloat(x);
		}

		public intToFloat(x: i32) -> bool { return true; }
		public intToFloat(x: f32) -> i32 { return 0; }
		""" + StdPreamble;

		(BoundCall call, BindingDiagnosticBag diagnostics) = BindFunctionBody(source, "caller");

		try
		{
			Assert.That(diagnostics.Diagnostics, Is.Empty);
			Assert.Multiple(() =>
			{
				Assert.That(call.Function.Name, Is.EqualTo("intToFloat"));
				Assert.That(call.Type.SpecialType, Is.EqualTo(SpecialType.StdBoolean));
			});
		}
		finally
		{
			diagnostics.Free();
		}
	}

	[Test]
	public void OverloadResolved_MultipleArgs_SelectsCorrectOverload()
	{
		const string source = """
		public caller(x: i8, y: i32) {
			let z = multi(x, y);
		}

		public multi(a: i8, b: i32) -> bool { return true; }
		public multi(a: i32, b: i32) -> i32 { return 0; }
		""" + StdPreamble;

		(BoundCall call, BindingDiagnosticBag diagnostics) = BindFunctionBody(source, "caller");

		try
		{
			Assert.That(diagnostics.Diagnostics, Is.Empty);
			Assert.Multiple(() =>
			{
				Assert.That(call.Function.Name, Is.EqualTo("multi"));
				Assert.That(call.Type.SpecialType, Is.EqualTo(SpecialType.StdBoolean));
			});
		}
		finally
		{
			diagnostics.Free();
		}
	}

	[Test]
	public void OverloadResolved_Ambiguous_ReportsError()
	{
		const string source = """
		public caller(a: i8, b: i8) {
			let y = ambiguous(a, b);
		}

		public ambiguous(x: i8, y: i32) -> bool { return true; }
		public ambiguous(x: i32, y: i8) -> i32 { return 0; }
		""" + StdPreamble;

		NiteCompilation compilation = CreateCompilation(source);
		Assert.That(compilation.GetDeclarationDiagnostics(), Is.Empty);

		FunctionSymbol function = compilation.SourceLibrary.GlobalModule
			.GetMembersUnordered()
			.OfType<FunctionSymbol>()
			.Single(f => f.Name == "caller");
		SourceFunctionSymbol sourceFunction = (SourceFunctionSymbol)function;

		Binder binder = sourceFunction.TryGetBodyBinder()!;
		BindingDiagnosticBag diagnostics = BindingDiagnosticBag.GetInstance();

		try
		{
			binder.BindFunctionBody(sourceFunction.Syntax, diagnostics);
			Assert.That(diagnostics.Diagnostics, Is.Not.Empty);
			string diagnosticIds = string.Join(", ", diagnostics.Diagnostics.Select(d => d.Id));
			Assert.That(diagnosticIds, Does.Contain("overload-resolution-failure"));
		}
		finally
		{
			diagnostics.Free();
		}
	}

	[Test]
	public void PreExistingTestStillPasses_WithStdPreamble()
	{
		const string source = """
		public caller(x: i32) {
			let y = callee(x);
		}

		public callee(x: i32) -> bool { return true; }
		""" + StdPreamble;

		(BoundCall call, BindingDiagnosticBag diagnostics) = BindFunctionBody(source, "caller");

		try
		{
			Assert.That(diagnostics.Diagnostics, Is.Empty);
			Assert.Multiple(() =>
			{
				Assert.That(call.Function.Name, Is.EqualTo("callee"));
				Assert.That(call.Arguments.Length, Is.EqualTo(1));
				Assert.That(call.Type.SpecialType, Is.EqualTo(SpecialType.StdBoolean));
			});
		}
		finally
		{
			diagnostics.Free();
		}
	}
}
