using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Compilation;

namespace NiteCompiler.Tests;

[TestFixture]
public class NiteParserTests
{
	// ==============================
	// Helpers
	// ==============================

	private static CompilationUnitSyntax ParseCompilationUnit(string source)
	{
		SyntaxTree tree = SyntaxTree.ParseText(source, "test.nite", NiteCompilationOptions.Default);
		return tree.Root;
	}

	private static TypeDeclarationSyntax ParseTypeDeclaration(string source)
	{
		SyntaxTree tree = SyntaxTree.ParseText(source, "test.nite", NiteCompilationOptions.Default);
		return tree.Root.Items.OfType<TypeDeclarationSyntax>().Single();
	}

	private static FunctionDeclarationSyntax ParseFunction(string source)
	{
		SyntaxTree tree = SyntaxTree.ParseText(source, "test.nite", NiteCompilationOptions.Default);
		return tree.Root.Items.OfType<FunctionDeclarationSyntax>().Single();
	}

	private static TStatement ParseSingleStatement<TStatement>(string source)
		where TStatement : StatementSyntax
	{
		SyntaxTree tree = SyntaxTree.ParseText(source, "test.nite", NiteCompilationOptions.Default);
		FunctionDeclarationSyntax function = tree.Root.Items
			.OfType<FunctionDeclarationSyntax>()
			.Single();
		BlockFunctionBodySyntax body = (BlockFunctionBodySyntax)function.Body;

		return (TStatement)body.Block.Statements.Single();
	}

	private static TExpression ParseSingleExpression<TExpression>(string source)
		where TExpression : ExpressionSyntax
	{
		ExpressionStatementSyntax stmt = ParseSingleStatement<ExpressionStatementSyntax>(source);
		return (TExpression)stmt.Expression;
	}

	// ==============================
	// Compilation Unit
	// ==============================

	[Test]
	public void Parse_EmptySource_HasNoItems()
	{
		CompilationUnitSyntax unit = ParseCompilationUnit("");
		Assert.Multiple(() =>
		{
			Assert.That(unit.Items.Count, Is.EqualTo(0));
			Assert.That(unit.Kind, Is.EqualTo(NodeKind.CompilationUnit));
			Assert.That(unit.EndOfFileToken.TKind, Is.EqualTo(TokenKind.EndOfFile));
		});
	}

	[Test]
	public void Parse_SingleFunction_CompilationUnitHasOneItem()
	{
		CompilationUnitSyntax unit = ParseCompilationUnit("public test() {}");
		Assert.That(unit.Items.Count, Is.EqualTo(1));
		Assert.That(unit.Items[0], Is.TypeOf<FunctionDeclarationSyntax>());
	}

	[Test]
	public void Parse_MultipleItems_CompilationUnitHasAllItems()
	{
		const string source = """
		public type Foo;
		public test() {}
		public type Bar;
		""";

		CompilationUnitSyntax unit = ParseCompilationUnit(source);
		Assert.That(unit.Items.Count, Is.EqualTo(3));
		Assert.That(unit.Items[0], Is.TypeOf<TypeDeclarationSyntax>());
		Assert.That(unit.Items[1], Is.TypeOf<FunctionDeclarationSyntax>());
		Assert.That(unit.Items[2], Is.TypeOf<TypeDeclarationSyntax>());
	}

	// ==============================
	// Function Declarations
	// ==============================

	[Test]
	public void Parse_FunctionNoBody_ReturnsEmptyBody()
	{
		const string source = "public test();";
		FunctionDeclarationSyntax func = ParseFunction(source);
		Assert.That(func.Body, Is.TypeOf<EmptyFunctionBodySyntax>());
	}

	[Test]
	public void Parse_FunctionWithBody_ReturnsBlockBody()
	{
		const string source = "public test() {}";
		FunctionDeclarationSyntax func = ParseFunction(source);
		Assert.Multiple(() =>
		{
			Assert.That(func.Body, Is.TypeOf<BlockFunctionBodySyntax>());
			Assert.That(func.Kind, Is.EqualTo(NodeKind.FunctionDeclaration));
		});
	}

	[Test]
	public void Parse_FunctionWithName_ReturnsCorrectName()
	{
		const string source = "public myFunc() {}";
		FunctionDeclarationSyntax func = ParseFunction(source);
		Assert.That(func.Name.GetName(), Is.EqualTo("myFunc"));
	}

	[Test]
	public void Parse_FunctionWithPublicAccessibility_HasPublicToken()
	{
		const string source = "public test() {}";
		FunctionDeclarationSyntax func = ParseFunction(source);
		Assert.That(func.AccessibilityToken.TKind, Is.EqualTo(TokenKind.Public));
	}

	[Test]
	public void Parse_FunctionWithNoAccessibility_HasNoneToken()
	{
		const string source = """
		public type Foo {
			test() {}
		}
		""";
		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		MembersTypeBodySyntax body = (MembersTypeBodySyntax)typeDecl.Body;
		FunctionDeclarationSyntax func = (FunctionDeclarationSyntax)body.Members[0];
		Assert.That(func.AccessibilityToken.TKind, Is.EqualTo(TokenKind.None));
	}

	[Test]
	public void Parse_FunctionWithPrivateAccessibility_HasPrivateToken()
	{
		const string source = "private test() {}";
		FunctionDeclarationSyntax func = ParseFunction(source);
		Assert.That(func.AccessibilityToken.TKind, Is.EqualTo(TokenKind.Private));
	}

	[Test]
	public void Parse_FunctionWithInternalAccessibility_HasInternalToken()
	{
		const string source = "internal test() {}";
		FunctionDeclarationSyntax func = ParseFunction(source);
		Assert.That(func.AccessibilityToken.TKind, Is.EqualTo(TokenKind.Internal));
	}

	[Test]
	public void Parse_FunctionWithProtectedAccessibility_HasProtectedToken()
	{
		const string source = "protected test() {}";
		FunctionDeclarationSyntax func = ParseFunction(source);
		Assert.That(func.AccessibilityToken.TKind, Is.EqualTo(TokenKind.Protected));
	}

	[Test]
	public void Parse_FunctionWithFriendAccessibility_HasFriendToken()
	{
		const string source = "friend test() {}";
		FunctionDeclarationSyntax func = ParseFunction(source);
		Assert.That(func.AccessibilityToken.TKind, Is.EqualTo(TokenKind.Friend));
	}

	[Test]
	public void Parse_FunctionWithFamilyAccessibility_HasFamilyToken()
	{
		const string source = "family test() {}";
		FunctionDeclarationSyntax func = ParseFunction(source);
		Assert.That(func.AccessibilityToken.TKind, Is.EqualTo(TokenKind.Family));
	}

	[Test]
	public void Parse_FunctionWithStaticModifier_HasStaticModifier()
	{
		const string source = "public static test() {}";
		FunctionDeclarationSyntax func = ParseFunction(source);
		Assert.That(func.Modifiers.Count, Is.EqualTo(1));
		Assert.That(func.Modifiers[0].TKind, Is.EqualTo(TokenKind.Static));
	}

	[Test]
	public void Parse_FunctionWithConstModifier_HasConstModifier()
	{
		const string source = "public const test() {}";
		FunctionDeclarationSyntax func = ParseFunction(source);
		Assert.That(func.Modifiers.Any(m => m.TKind == TokenKind.Const));
	}

	[Test]
	public void Parse_FunctionWithPureModifier_HasPureModifier()
	{
		const string source = "public pure test() {}";
		FunctionDeclarationSyntax func = ParseFunction(source);
		Assert.That(func.Modifiers.Any(m => m.TKind == TokenKind.Pure));
	}

	[Test]
	public void Parse_FunctionWithMultipleModifiers_HasAllModifiers()
	{
		const string source = "public static pure test() {}";
		FunctionDeclarationSyntax func = ParseFunction(source);
		Assert.That(func.Modifiers.Count, Is.EqualTo(2));
		Assert.That(func.Modifiers[0].TKind, Is.EqualTo(TokenKind.Static));
		Assert.That(func.Modifiers[1].TKind, Is.EqualTo(TokenKind.Pure));
	}

	[Test]
	public void Parse_FunctionWithParameter_HasParameterList()
	{
		const string source = "public test(x: i32) {}";
		FunctionDeclarationSyntax func = ParseFunction(source);
		Assert.Multiple(() =>
		{
			Assert.That(func.ParameterList.OpenParen.TKind, Is.EqualTo(TokenKind.OpenParen));
			Assert.That(func.ParameterList.CloseParen.TKind, Is.EqualTo(TokenKind.CloseParen));
			Assert.That(func.ParameterList.Parameters.Count, Is.EqualTo(1));
		});
	}

	[Test]
	public void Parse_FunctionWithMultipleParameters_HasAllParameters()
	{
		const string source = "public test(x: i32, y: bool, z: f64) {}";
		FunctionDeclarationSyntax func = ParseFunction(source);
		Assert.That(func.ParameterList.Parameters.Count, Is.EqualTo(3));
	}

	[Test]
	public void Parse_FunctionWithNoParameters_HasEmptyParameterList()
	{
		const string source = "public test() {}";
		FunctionDeclarationSyntax func = ParseFunction(source);
		Assert.That(func.ParameterList.Parameters.Count, Is.EqualTo(0));
	}

	[Test]
	public void Parse_FunctionWithParameter_ReadsParameterName()
	{
		const string source = "public test(myParam: i32) {}";
		FunctionDeclarationSyntax func = ParseFunction(source);
		ParameterSyntax param = (ParameterSyntax)func.ParameterList.Parameters[0];
		Assert.That(param.Name.GetName(), Is.EqualTo("myParam"));
	}

	[Test]
	public void Parse_FunctionWithParameter_ReadsParameterType()
	{
		const string source = "public test(x: bool) {}";
		FunctionDeclarationSyntax func = ParseFunction(source);
		ParameterSyntax param = (ParameterSyntax)func.ParameterList.Parameters[0];
		Assert.That(param.TypeClauseSyntax.Type, Is.TypeOf<PredefinedTypeSyntax>());
		PredefinedTypeSyntax pType = (PredefinedTypeSyntax)param.TypeClauseSyntax.Type;
		Assert.That(pType.TypeKeyword.TKind, Is.EqualTo(TokenKind.Boolean));
	}

	[Test]
	public void Parse_FunctionWithReturnType_HasReturnTypeClause()
	{
		const string source = "public test() -> i32 {}";
		FunctionDeclarationSyntax func = ParseFunction(source);
		Assert.Multiple(() =>
		{
			Assert.That(func.ReturnTypeClause, Is.Not.Null);
			Assert.That(func.ReturnTypeClause!.Token.TKind, Is.EqualTo(TokenKind.Retusa));
			Assert.That(func.ReturnTypeClause.Type, Is.TypeOf<PredefinedTypeSyntax>());
		});
	}

	[Test]
	public void Parse_FunctionWithNoReturnType_HasNoReturnTypeClause()
	{
		const string source = "public test() {}";
		FunctionDeclarationSyntax func = ParseFunction(source);
		Assert.That(func.ReturnTypeClause, Is.Null);
	}

	[Test]
	public void Parse_FunctionWithVoidReturn_HasReturnType()
	{
		const string source = "public test() -> void {}";
		FunctionDeclarationSyntax func = ParseFunction(source);
		Assert.That(func.ReturnTypeClause, Is.Not.Null);
	}

	[Test]
	public void Parse_FunctionWithNeverReturn_HasReturnType()
	{
		const string source = "public test() -> void {}";
		FunctionDeclarationSyntax func = ParseFunction(source);
		Assert.That(func.ReturnTypeClause, Is.Not.Null);
	}

	[Test]
	public void Parse_FunctionBody_ContainsBlockStatements()
	{
		const string source = """
		public test() {
			foo;
		}
		""";
		FunctionDeclarationSyntax func = ParseFunction(source);
		BlockFunctionBodySyntax body = (BlockFunctionBodySyntax)func.Body;
		Assert.That(body.Block.Statements.Count, Is.EqualTo(1));
		Assert.That(body.Block.Statements[0], Is.TypeOf<ExpressionStatementSyntax>());
	}

	// ==============================
	// Type Declarations
	// ==============================

	[Test]
	public void Parse_TypeWithEmptyBody_ParsesSemicolon()
	{
		const string source = "public type Foo;";
		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		Assert.That(typeDecl.Body, Is.TypeOf<EmptyTypeBodySyntax>());
	}

	[Test]
	public void Parse_TypeWithBody_ParsesMembersBody()
	{
		const string source = "public type Foo {}";
		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		Assert.That(typeDecl.Body, Is.TypeOf<MembersTypeBodySyntax>());
	}

	[Test]
	public void Parse_TypeWithName_ReturnsCorrectName()
	{
		const string source = "public type MyType;";
		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		Assert.That(typeDecl.Name.GetName(), Is.EqualTo("MyType"));
	}

	[Test]
	public void Parse_TypeWithPublicAccessibility_HasPublicToken()
	{
		const string source = "public type Foo;";
		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		Assert.That(typeDecl.AccessibilityToken.TKind, Is.EqualTo(TokenKind.Public));
	}

	[Test]
	public void Parse_TypeWithPrivateAccessibility_HasPrivateToken()
	{
		const string source = "private type Foo;";
		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		Assert.That(typeDecl.AccessibilityToken.TKind, Is.EqualTo(TokenKind.Private));
	}

	[Test]
	public void Parse_TypeWithNoAccessibility_HasNoneToken()
	{
		const string source = "type Foo;";
		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		Assert.That(typeDecl.AccessibilityToken.TKind, Is.EqualTo(TokenKind.None));
	}

	[Test]
	public void Parse_TypeWithPartialModifier_HasPartialModifier()
	{
		const string source = "public partial type Foo;";
		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		Assert.That(typeDecl.Modifiers.Any(m => m.TKind == TokenKind.Partial));
	}

	[Test]
	public void Parse_TypeWithUnsizedModifier_HasUnsizedModifier()
	{
		const string source = "public unsized type Foo;";
		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		Assert.That(typeDecl.Modifiers.Any(m => m.TKind == TokenKind.Unsized));
	}

	[Test]
	public void Parse_TypeBodyWithField_ParsesFieldDeclaration()
	{
		const string source = """
		public type Foo {
			public x: i32;
		}
		""";

		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		MembersTypeBodySyntax body = (MembersTypeBodySyntax)typeDecl.Body;

		Assert.Multiple(() =>
		{
			Assert.That(body.Members.Count, Is.EqualTo(1));
			Assert.That(body.Members[0], Is.TypeOf<FieldDeclarationSyntax>());
			FieldDeclarationSyntax field = (FieldDeclarationSyntax)body.Members[0];
			Assert.That(field.Name.GetName(), Is.EqualTo("x"));
			Assert.That(field.AccessibilityToken.TKind, Is.EqualTo(TokenKind.Public));
		});
	}

	[Test]
	public void Parse_TypeBodyWithMethod_ParsesFunctionDeclaration()
	{
		const string source = """
		public type Foo {
			public test() {}
		}
		""";

		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		MembersTypeBodySyntax body = (MembersTypeBodySyntax)typeDecl.Body;

		Assert.Multiple(() =>
		{
			Assert.That(body.Members.Count, Is.EqualTo(1));
			Assert.That(body.Members[0], Is.TypeOf<FunctionDeclarationSyntax>());
		});
	}

	[Test]
	public void Parse_TypeBodyWithNestedType_ParsesTypeDeclaration()
	{
		const string source = """
		public type Foo {
			public type Bar;
		}
		""";

		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		MembersTypeBodySyntax body = (MembersTypeBodySyntax)typeDecl.Body;

		Assert.Multiple(() =>
		{
			Assert.That(body.Members.Count, Is.EqualTo(1));
			Assert.That(body.Members[0], Is.TypeOf<TypeDeclarationSyntax>());
			TypeDeclarationSyntax nested = (TypeDeclarationSyntax)body.Members[0];
			Assert.That(nested.Name.GetName(), Is.EqualTo("Bar"));
			Assert.That(nested.Body, Is.TypeOf<EmptyTypeBodySyntax>());
		});
	}

	[Test]
	public void Parse_TypeBodyWithConstructor_ParsesConstructor()
	{
		const string source = """
		public type Foo {
			public new() {}
		}
		""";

		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		MembersTypeBodySyntax body = (MembersTypeBodySyntax)typeDecl.Body;

		Assert.That(body.Members.Count, Is.EqualTo(1));
		Assert.That(body.Members[0], Is.TypeOf<ConstructorDeclarationSyntax>());
	}

	[Test]
	public void Parse_TypeBodyWithNamedConstructor_ParsesNamedConstructor()
	{
		const string source = """
		public type Foo {
			public new create() {}
		}
		""";

		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		MembersTypeBodySyntax body = (MembersTypeBodySyntax)typeDecl.Body;

		Assert.Multiple(() =>
		{
			Assert.That(body.Members.Count, Is.EqualTo(1));
			Assert.That(body.Members[0], Is.TypeOf<NamedConstructorDeclarationSyntax>());
			NamedConstructorDeclarationSyntax nc = (NamedConstructorDeclarationSyntax)body.Members[0];
			Assert.That(nc.Name.GetName(), Is.EqualTo("create"));
		});
	}

	[Test]
	public void Parse_TypeBodyWithMixedMembers_ParsesAllKinds()
	{
		const string source = """
		public type Foo {
			public x: i32;
			public test() {}
			public type Bar;
		}
		""";

		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		MembersTypeBodySyntax body = (MembersTypeBodySyntax)typeDecl.Body;

		Assert.Multiple(() =>
		{
			Assert.That(body.Members.Count, Is.EqualTo(3));
			Assert.That(body.Members[0], Is.TypeOf<FieldDeclarationSyntax>());
			Assert.That(body.Members[1], Is.TypeOf<FunctionDeclarationSyntax>());
			Assert.That(body.Members[2], Is.TypeOf<TypeDeclarationSyntax>());
		});
	}

	[Test]
	public void Parse_TypeBodyWithFieldWithoutAccessibility_StillParsesField()
	{
		const string source = """
		public type Foo {
			x: i32;
		}
		""";

		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		MembersTypeBodySyntax body = (MembersTypeBodySyntax)typeDecl.Body;

		Assert.Multiple(() =>
		{
			Assert.That(body.Members.Count, Is.EqualTo(1));
			Assert.That(body.Members[0], Is.TypeOf<FieldDeclarationSyntax>());
			FieldDeclarationSyntax field = (FieldDeclarationSyntax)body.Members[0];
			Assert.That(field.Name.GetName(), Is.EqualTo("x"));
			Assert.That(field.AccessibilityToken.TKind, Is.EqualTo(TokenKind.None));
		});
	}

	[Test]
	public void Parse_TypeBodyWithMethodWithoutAccessibility_StillParsesMethod()
	{
		const string source = """
		public type Foo {
			test() {}
		}
		""";

		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		MembersTypeBodySyntax body = (MembersTypeBodySyntax)typeDecl.Body;

		Assert.Multiple(() =>
		{
			Assert.That(body.Members.Count, Is.EqualTo(1));
			Assert.That(body.Members[0], Is.TypeOf<FunctionDeclarationSyntax>());
		});
	}

	[Test]
	public void Parse_TypeBodyWithMultipleFields_ParsesAllFields()
	{
		const string source = """
		public type Foo {
			public a: i32;
			public b: bool;
			public c: f64;
		}
		""";

		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		MembersTypeBodySyntax body = (MembersTypeBodySyntax)typeDecl.Body;

		Assert.Multiple(() =>
		{
			Assert.That(body.Members.Count, Is.EqualTo(3));
			Assert.That(((FieldDeclarationSyntax)body.Members[0]).Name.GetName(), Is.EqualTo("a"));
			Assert.That(((FieldDeclarationSyntax)body.Members[1]).Name.GetName(), Is.EqualTo("b"));
			Assert.That(((FieldDeclarationSyntax)body.Members[2]).Name.GetName(), Is.EqualTo("c"));
		});
	}

	[Test]
	public void Parse_TypeWithGenericParameter_ParsesGenericParam()
	{
		const string source = "public type Foo<T> {}";
		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		Assert.That(typeDecl.GenericParameterList, Is.Not.Null);
		Assert.That(typeDecl.GenericParameterList!.Parameters.Count, Is.EqualTo(1));
		Assert.That(typeDecl.GenericParameterList.Parameters[0], Is.TypeOf<GenericTypeParameterSyntax>());
	}

	[Test]
	public void Parse_TypeWithMultipleGenericParams_ParsesAll()
	{
		const string source = "public type Foo<T, U, V> {}";
		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		Assert.That(typeDecl.GenericParameterList!.Parameters.Count, Is.EqualTo(3));
	}

	// ==============================
	// Field Declarations (top-level)
	// ==============================

	[Test]
	public void Parse_TopLevelField_ParsesField()
	{
		const string source = "public x: i32;";
		SyntaxTree tree = SyntaxTree.ParseText(source, "test.nite", NiteCompilationOptions.Default);
		Assert.That(tree.Root.Items.Count, Is.EqualTo(1));
		Assert.That(tree.Root.Items[0], Is.TypeOf<FieldDeclarationSyntax>());
		FieldDeclarationSyntax field = (FieldDeclarationSyntax)tree.Root.Items[0];
		Assert.That(field.Name.GetName(), Is.EqualTo("x"));
		Assert.That(field.TypeClause.Type.Kind, Is.EqualTo(NodeKind.PredefinedType));
	}

	[Test]
	public void Parse_TopLevelFieldWithPrivateAccessibility_ParsesField()
	{
		const string source = "private x: i32;";
		SyntaxTree tree = SyntaxTree.ParseText(source, "test.nite", NiteCompilationOptions.Default);
		FieldDeclarationSyntax field = (FieldDeclarationSyntax)tree.Root.Items[0];
		Assert.That(field.AccessibilityToken.TKind, Is.EqualTo(TokenKind.Private));
	}

	// ==============================
	// Module Declaration
	// ==============================

	[Test]
	public void Parse_ModuleDeclaration_ParsesModule()
	{
		const string source = """
		module foo;
		public test() {}
		""";
		CompilationUnitSyntax unit = ParseCompilationUnit(source);
		Assert.That(unit.Items.Count, Is.EqualTo(1)); // module wraps the test() inside it
		Assert.That(unit.Items[0], Is.TypeOf<ModuleDeclarationSyntax>());
	}

	[Test]
	public void Parse_ModuleWithName_ParsesCorrectName()
	{
		const string source = """
		module my_module;
		public test() {}
		""";
		CompilationUnitSyntax unit = ParseCompilationUnit(source);
		ModuleDeclarationSyntax module = (ModuleDeclarationSyntax)unit.Items[0];
		Assert.That(module.Name.GetName(), Is.EqualTo("my_module"));
	}

	[Test]
	public void Parse_ModuleWithMembers_HasMembers()
	{
		const string source = """
		module foo;
		public test() {}
		public type Bar;
		""";
		CompilationUnitSyntax unit = ParseCompilationUnit(source);
		ModuleDeclarationSyntax module = (ModuleDeclarationSyntax)unit.Items[0];
		Assert.That(module.Members.Count, Is.EqualTo(2));
	}

	// ==============================
	// Constructor Declarations
	// ==============================

	[Test]
	public void Parse_ConstructorWithBody_ParsesBlockBody()
	{
		const string source = """
		public type Foo {
			public new() {}
		}
		""";
		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		MembersTypeBodySyntax body = (MembersTypeBodySyntax)typeDecl.Body;
		ConstructorDeclarationSyntax ctor = (ConstructorDeclarationSyntax)body.Members[0];
		Assert.That(ctor.Body, Is.TypeOf<BlockFunctionBodySyntax>());
	}

	[Test]
	public void Parse_ConstructorWithParameters_ParsesParameters()
	{
		const string source = """
		public type Foo {
			public new(x: i32, y: bool) {}
		}
		""";
		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		MembersTypeBodySyntax body = (MembersTypeBodySyntax)typeDecl.Body;
		ConstructorDeclarationSyntax ctor = (ConstructorDeclarationSyntax)body.Members[0];
		Assert.That(ctor.ParameterList.Parameters.Count, Is.EqualTo(2));
	}

	[Test]
	public void Parse_ConstructorWithEmptyBody_ParsesSemicolon()
	{
		const string source = """
		public type Foo {
			public new();
		}
		""";
		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		MembersTypeBodySyntax body = (MembersTypeBodySyntax)typeDecl.Body;
		ConstructorDeclarationSyntax ctor = (ConstructorDeclarationSyntax)body.Members[0];
		Assert.That(ctor.Body, Is.TypeOf<EmptyFunctionBodySyntax>());
	}

	[Test]
	public void Parse_NamedConstructor_ParsesWithName()
	{
		const string source = """
		public type Foo {
			public new from_i32(x: i32) {}
		}
		""";
		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		MembersTypeBodySyntax body = (MembersTypeBodySyntax)typeDecl.Body;
		NamedConstructorDeclarationSyntax nc = (NamedConstructorDeclarationSyntax)body.Members[0];
		Assert.That(nc.Name.GetName(), Is.EqualTo("from_i32"));
		Assert.That(nc.ParameterList.Parameters.Count, Is.EqualTo(1));
	}

	[Test]
	public void Parse_NamedConstructorWithNoAccessibility_ParsesDefaultAccess()
	{
		const string source = """
		public type Foo {
			new from_default() {}
		}
		""";
		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		MembersTypeBodySyntax body = (MembersTypeBodySyntax)typeDecl.Body;
		NamedConstructorDeclarationSyntax nc = (NamedConstructorDeclarationSyntax)body.Members[0];
		Assert.That(nc.AccessibilityToken.TKind, Is.EqualTo(TokenKind.None));
	}

	// ==============================
	// Expression Statements
	// ==============================

	[Test]
	public void Parse_ExpressionStatement_HasSemicolon()
	{
		const string source = """
		public test() {
			foo;
		}
		""";
		ExpressionStatementSyntax stmt = ParseSingleStatement<ExpressionStatementSyntax>(source);
		Assert.That(stmt.SemicolonToken.TKind, Is.EqualTo(TokenKind.Semicolon));
	}

	[Test]
	public void Parse_ExpressionStatement_ExpressionIsIdentifier()
	{
		const string source = """
		public test() {
			foo;
		}
		""";
		ExpressionStatementSyntax stmt = ParseSingleStatement<ExpressionStatementSyntax>(source);
		Assert.That(stmt.Expression, Is.TypeOf<IdentifierNameSyntax>());
	}

	// ==============================
	// Block Statements
	// ==============================

	[Test]
	public void Parse_BlockStatement_HasBracesAndStatements()
	{
		const string source = """
		public test() {
			{ foo; bar; }
		}
		""";
		BlockStatementSyntax block = ParseSingleStatement<BlockStatementSyntax>(source);
		Assert.Multiple(() =>
		{
			Assert.That(block.OpenBrace.TKind, Is.EqualTo(TokenKind.OpenBrace));
			Assert.That(block.CloseBrace.TKind, Is.EqualTo(TokenKind.CloseBrace));
			Assert.That(block.Statements.Count, Is.EqualTo(2));
		});
	}

	[Test]
	public void Parse_EmptyBlockStatement_HasNoStatements()
	{
		const string source = """
		public test() {
			{}
		}
		""";
		BlockStatementSyntax block = ParseSingleStatement<BlockStatementSyntax>(source);
		Assert.That(block.Statements.Count, Is.EqualTo(0));
	}

	// ==============================
	// Empty Statements
	// ==============================

	[Test]
	public void Parse_EmptyStatement_HasSemicolon()
	{
		const string source = """
		public test() {
			;
		}
		""";
		ErrorStatementSyntax stmt = ParseSingleStatement<ErrorStatementSyntax>(source);
		Assert.That(stmt.ErrorNodes, Is.Not.Empty);
	}

	// ==============================
	// Return Statements
	// ==============================

	[Test]
	public void Parse_ReturnStatement_HasReturnKeyword()
	{
		const string source = """
		public test() {
			return;
		}
		""";
		ReturnStatementSyntax stmt = ParseSingleStatement<ReturnStatementSyntax>(source);
		Assert.Multiple(() =>
		{
			Assert.That(stmt.ReturnKeyword.TKind, Is.EqualTo(TokenKind.Return));
			Assert.That(stmt.Expression, Is.Null);
			Assert.That(stmt.SemicolonToken.TKind, Is.EqualTo(TokenKind.Semicolon));
		});
	}

	[Test]
	public void Parse_ReturnStatementWithValue_HasExpression()
	{
		const string source = """
		public test() -> i32 {
			return 42;
		}
		""";
		ReturnStatementSyntax stmt = ParseSingleStatement<ReturnStatementSyntax>(source);
		Assert.That(stmt.Expression, Is.Not.Null);
		Assert.That(stmt.Expression, Is.TypeOf<LiteralExpressionSyntax>());
	}

	[Test]
	public void Parse_ReturnStatementWithIdentifier_HasIdentifierExpression()
	{
		const string source = """
		public test() -> i32 {
			return result;
		}
		""";
		ReturnStatementSyntax stmt = ParseSingleStatement<ReturnStatementSyntax>(source);
		Assert.That(stmt.Expression, Is.TypeOf<IdentifierNameSyntax>());
		IdentifierNameSyntax id = (IdentifierNameSyntax)stmt.Expression;
		Assert.That(id.Identifier.TKind, Is.EqualTo(TokenKind.IdentifierOrKeyword));
	}

	// ==============================
	// If Statements
	// ==============================

	[Test]
	public void Parse_IfStatement_HasIfTokenConditionAndBody()
	{
		const string source = """
		public test() {
			if true {}
		}
		""";
		IfStatementSyntax stmt = ParseSingleStatement<IfStatementSyntax>(source);
		Assert.Multiple(() =>
		{
			Assert.That(stmt.IfToken.TKind, Is.EqualTo(TokenKind.If));
			Assert.That(stmt.Condition, Is.TypeOf<LiteralExpressionSyntax>());
			Assert.That(stmt.ThenStatement, Is.TypeOf<BlockStatementSyntax>());
			Assert.That(stmt.ElseClause, Is.Null);
		});
	}

	[Test]
	public void Parse_IfElseStatement_HasElseClause()
	{
		const string source = """
		public test() {
			if true {} else {}
		}
		""";
		IfStatementSyntax stmt = ParseSingleStatement<IfStatementSyntax>(source);
		Assert.That(stmt.ElseClause, Is.Not.Null);
		Assert.That(stmt.ElseClause!.ElseToken.TKind, Is.EqualTo(TokenKind.Else));
		Assert.That(stmt.ElseClause.ElseStatement, Is.TypeOf<BlockStatementSyntax>());
	}

	[Test]
	public void Parse_IfElseIfStatement_ChainsCorrectly()
	{
		const string source = """
		public test() {
			if true {} else if false {} else {}
		}
		""";
		IfStatementSyntax stmt = ParseSingleStatement<IfStatementSyntax>(source);
		Assert.That(stmt.ElseClause, Is.Not.Null);
		Assert.That(stmt.ElseClause!.ElseStatement, Is.TypeOf<IfStatementSyntax>());
	}

	[Test]
	public void Parse_IfStatementWithParenthesizedCondition_HasParens()
	{
		const string source = """
		public test() {
			if (x > 0) {}
		}
		""";
		IfStatementSyntax stmt = ParseSingleStatement<IfStatementSyntax>(source);
		Assert.That(stmt.Condition, Is.TypeOf<ParenthesizedExpressionSyntax>());
	}

	// ==============================
	// Loop Statements
	// ==============================

	[Test]
	public void Parse_LoopStatement_HasLoopKeywordAndBody()
	{
		const string source = """
		public test() {
			loop {}
		}
		""";
		LoopStatementSyntax stmt = ParseSingleStatement<LoopStatementSyntax>(source);
		Assert.Multiple(() =>
		{
			Assert.That(stmt.LoopKeyword.TKind, Is.EqualTo(TokenKind.Loop));
			Assert.That(stmt.Body, Is.TypeOf<BlockStatementSyntax>());
		});
	}

	[Test]
	public void Parse_LoopStatementWithSingleStatement_HasStatementBody()
	{
		const string source = """
		public test() {
			loop foo();
		}
		""";
		LoopStatementSyntax stmt = ParseSingleStatement<LoopStatementSyntax>(source);
		Assert.That(stmt.Body, Is.TypeOf<ExpressionStatementSyntax>());
	}

	// ==============================
	// While Statements
	// ==============================

	[Test]
	public void Parse_WhileStatement_HasWhileKeywordConditionAndBody()
	{
		const string source = """
		public test() {
			while true {}
		}
		""";
		WhileStatementSyntax stmt = ParseSingleStatement<WhileStatementSyntax>(source);
		Assert.Multiple(() =>
		{
			Assert.That(stmt.WhileKeyword.TKind, Is.EqualTo(TokenKind.While));
			Assert.That(stmt.Condition, Is.TypeOf<LiteralExpressionSyntax>());
			Assert.That(stmt.Body, Is.TypeOf<BlockStatementSyntax>());
		});
	}

	[Test]
	public void Parse_WhileStatementWithComparisonCondition_HasBinaryExpression()
	{
		const string source = """
		public test() {
			while x < 10 {}
		}
		""";
		WhileStatementSyntax stmt = ParseSingleStatement<WhileStatementSyntax>(source);
		Assert.That(stmt.Condition, Is.TypeOf<BinaryExpressionSyntax>());
	}

	// ==============================
	// Break Statement
	// ==============================

	[Test]
	public void Parse_BreakStatement_IsExpressionStatement()
	{
		// break may be parsed as an expression or a statement
		// Let's check if it parses at all
		const string source = """
		public test() {
			break;
		}
		""";
		SyntaxTree tree = SyntaxTree.ParseText(source, "test.nite", NiteCompilationOptions.Default);
		Assert.DoesNotThrow(() => _ = tree.Root.ToString());
	}

	// ==============================
	// Local Variable Declaration
	// ==============================

	[Test]
	public void Parse_LetStatement_HasLetTokenAndDeclarator()
	{
		const string source = """
		public test() {
			let x = 42;
		}
		""";
		LocalVariableDeclarationStatement stmt = ParseSingleStatement<LocalVariableDeclarationStatement>(source);
		Assert.Multiple(() =>
		{
			Assert.That(stmt.DeclarationToken.TKind, Is.EqualTo(TokenKind.Let));
			Assert.That(stmt.SemicolonToken.TKind, Is.EqualTo(TokenKind.Semicolon));
		});
	}

	[Test]
	public void Parse_LetStatementWithName_ParsesName()
	{
		const string source = """
		public test() {
			let myVar = 42;
		}
		""";
		LocalVariableDeclarationStatement stmt = ParseSingleStatement<LocalVariableDeclarationStatement>(source);
		Assert.That(stmt.Declarator.Name.GetName(), Is.EqualTo("myVar"));
	}

	[Test]
	public void Parse_LetStatementWithType_ParsesType()
	{
		const string source = """
		public test() {
			let x: i32 = 42;
		}
		""";
		LocalVariableDeclarationStatement stmt = ParseSingleStatement<LocalVariableDeclarationStatement>(source);
		Assert.That(stmt.Declarator.TypeClause, Is.Not.Null);
		Assert.That(stmt.Declarator.TypeClause!.Type, Is.TypeOf<PredefinedTypeSyntax>());
	}

	[Test]
	public void Parse_LetStatementWithInitializer_ParsesEqualsValue()
	{
		const string source = """
		public test() {
			let x = 42;
		}
		""";
		LocalVariableDeclarationStatement stmt = ParseSingleStatement<LocalVariableDeclarationStatement>(source);
		Assert.That(stmt.Declarator.EqualsValueClause, Is.Not.Null);
		Assert.That(stmt.Declarator.EqualsValueClause!.EqualsToken.TKind, Is.EqualTo(TokenKind.Equal));
		Assert.That(stmt.Declarator.EqualsValueClause.Expression, Is.TypeOf<LiteralExpressionSyntax>());
	}

	[Test]
	public void Parse_LetStatementNoInitializer_ParsesWithoutEqualsValue()
	{
		const string source = """
		public test() {
			let x: i32;
		}
		""";
		LocalVariableDeclarationStatement stmt = ParseSingleStatement<LocalVariableDeclarationStatement>(source);
		Assert.That(stmt.Declarator.EqualsValueClause, Is.Null);
		Assert.That(stmt.Declarator.TypeClause, Is.Not.Null);
	}

	[Test]
	public void Parse_LetStatementWithBoolType_ParsesPredefinedType()
	{
		const string source = """
		public test() {
			let x: bool = true;
		}
		""";
		LocalVariableDeclarationStatement stmt = ParseSingleStatement<LocalVariableDeclarationStatement>(source);
		PredefinedTypeSyntax pType = (PredefinedTypeSyntax)stmt.Declarator.TypeClause!.Type;
		Assert.That(pType.TypeKeyword.TKind, Is.EqualTo(TokenKind.Boolean));
	}

	[Test]
	public void Parse_LetStatementWithOptionalType_ParsesReferenceType()
	{
		const string source = """
		public test() {
			let x: &i32 = &y;
		}
		""";
		LocalVariableDeclarationStatement stmt = ParseSingleStatement<LocalVariableDeclarationStatement>(source);
		Assert.That(stmt.Declarator.TypeClause!.Type, Is.TypeOf<ReferenceTypeSyntax>());
	}

	// ==============================
	// Literal Expressions
	// ==============================

	[Test]
	public void Parse_TrueLiteral_ReturnsTrueLiteralExpression()
	{
		LiteralExpressionSyntax expr = ParseSingleExpression<LiteralExpressionSyntax>("public test() { true; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.TrueLiteralExpression));
		Assert.That(expr.Token.TKind, Is.EqualTo(TokenKind.True));
	}

	[Test]
	public void Parse_FalseLiteral_ReturnsFalseLiteralExpression()
	{
		LiteralExpressionSyntax expr = ParseSingleExpression<LiteralExpressionSyntax>("public test() { false; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.FalseLiteralExpression));
		Assert.That(expr.Token.TKind, Is.EqualTo(TokenKind.False));
	}

	[Test]
	public void Parse_NumberLiteral_ReturnsNumberLiteralExpression()
	{
		LiteralExpressionSyntax expr = ParseSingleExpression<LiteralExpressionSyntax>("public test() { 42; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.NumberLiteralExpression));
		Assert.That(expr.Token.TKind, Is.EqualTo(TokenKind.NumberLiteral));
	}

	[Test]
	public void Parse_StringLiteral_ReturnsStringLiteralExpression()
	{
		LiteralExpressionSyntax expr = ParseSingleExpression<LiteralExpressionSyntax>("public test() { \"hello\"; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.StringLiteralExpression));
		Assert.That(expr.Token.TKind, Is.EqualTo(TokenKind.StringLiteral));
	}

	[Test]
	public void Parse_CharacterLiteral_ReturnsCharacterLiteralExpression()
	{
		LiteralExpressionSyntax expr = ParseSingleExpression<LiteralExpressionSyntax>("public test() { 'a'; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.CharacterLiteralExpression));
		Assert.That(expr.Token.TKind, Is.EqualTo(TokenKind.CharacterLiteral));
	}

	// ==============================
	// Self Expression
	// ==============================

	[Test]
	public void Parse_SelfExpression_ReturnsSelfExpression()
	{
		SelfExpressionSyntax expr = ParseSingleExpression<SelfExpressionSyntax>("public test() { self; }");
		Assert.That(expr.SelfKeyword.TKind, Is.EqualTo(TokenKind.Self));
	}

	// ==============================
	// Parenthesized Expression
	// ==============================

	[Test]
	public void Parse_ParenthesizedExpression_HasParensAndInner()
	{
		ParenthesizedExpressionSyntax expr = ParseSingleExpression<ParenthesizedExpressionSyntax>("public test() { (42); }");
		Assert.Multiple(() =>
		{
			Assert.That(expr.OpenParenToken.TKind, Is.EqualTo(TokenKind.OpenParen));
			Assert.That(expr.CloseParenToken.TKind, Is.EqualTo(TokenKind.CloseParen));
			Assert.That(expr.Expression, Is.TypeOf<LiteralExpressionSyntax>());
		});
	}

	[Test]
	public void Parse_NestedParentheses_AllNested()
	{
		ParenthesizedExpressionSyntax outer = ParseSingleExpression<ParenthesizedExpressionSyntax>("public test() { ((42)); }");
		Assert.That(outer.Expression, Is.TypeOf<ParenthesizedExpressionSyntax>());
	}

	// ==============================
	// Unary Expressions
	// ==============================

	[Test]
	public void Parse_UnaryMinus_ReturnsUnaryExpression()
	{
		UnaryExpressionSyntax expr = ParseSingleExpression<UnaryExpressionSyntax>("public test() { -x; }");
		Assert.Multiple(() =>
		{
			Assert.That(expr.Kind, Is.EqualTo(NodeKind.UnarySubtractExpression));
			Assert.That(expr.Operator.TKind, Is.EqualTo(TokenKind.Minus));
			Assert.That(expr.Expression, Is.TypeOf<IdentifierNameSyntax>());
		});
	}

	[Test]
	public void Parse_UnaryPlus_ReturnsUnaryExpression()
	{
		UnaryExpressionSyntax expr = ParseSingleExpression<UnaryExpressionSyntax>("public test() { +x; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.UnaryAddExpression));
		Assert.That(expr.Operator.TKind, Is.EqualTo(TokenKind.Plus));
	}

	[Test]
	public void Parse_UnaryNot_ReturnsUnaryExpression()
	{
		UnaryExpressionSyntax expr = ParseSingleExpression<UnaryExpressionSyntax>("public test() { !x; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.UnaryLogicalNotExpression));
		Assert.That(expr.Operator.TKind, Is.EqualTo(TokenKind.ExclamationSign));
	}

	[Test]
	public void Parse_UnaryTilde_ReturnsUnaryExpression()
	{
		UnaryExpressionSyntax expr = ParseSingleExpression<UnaryExpressionSyntax>("public test() { ~x; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.UnaryTildeExpression));
		Assert.That(expr.Operator.TKind, Is.EqualTo(TokenKind.Tilde));
	}

	[Test]
	public void Parse_AddressOfExpression_ReturnsUnaryExpression()
	{
		UnaryExpressionSyntax expr = ParseSingleExpression<UnaryExpressionSyntax>("public test() { &x; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.AddressOfExpression));
		Assert.That(expr.Operator.TKind, Is.EqualTo(TokenKind.Ampersand));
	}

	[Test]
	public void Parse_DereferenceExpression_ReturnsUnaryExpression()
	{
		UnaryExpressionSyntax expr = ParseSingleExpression<UnaryExpressionSyntax>("public test() { *x; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.DereferencingExpression));
		Assert.That(expr.Operator.TKind, Is.EqualTo(TokenKind.Asterisk));
	}

	[Test]
	public void Parse_DoubleUnary_DoubleNegation()
	{
		const string source = """
		public test() {
			--x;
		}
		""";
		SyntaxTree tree = SyntaxTree.ParseText(source, "test.nite", NiteCompilationOptions.Default);
		FunctionDeclarationSyntax func = tree.Root.Items.OfType<FunctionDeclarationSyntax>().Single();
		BlockFunctionBodySyntax body = (BlockFunctionBodySyntax)func.Body;
		ExpressionStatementSyntax stmt = (ExpressionStatementSyntax)body.Block.Statements.Single();

		// Should be unary minus of a unary minus expression
		Assert.That(stmt.Expression, Is.TypeOf<UnaryExpressionSyntax>());
		UnaryExpressionSyntax outer = (UnaryExpressionSyntax)stmt.Expression;
		Assert.That(outer.Expression, Is.TypeOf<UnaryExpressionSyntax>());
	}

	// ==============================
	// Binary Expressions
	// ==============================

	[Test]
	public void Parse_AddExpression_ReturnsBinaryExpression()
	{
		BinaryExpressionSyntax expr = ParseSingleExpression<BinaryExpressionSyntax>("public test() { 1 + 2; }");
		Assert.Multiple(() =>
		{
			Assert.That(expr.Kind, Is.EqualTo(NodeKind.AddExpression));
			Assert.That(expr.OperatorToken.TKind, Is.EqualTo(TokenKind.Plus));
			Assert.That(expr.Left, Is.TypeOf<LiteralExpressionSyntax>());
			Assert.That(expr.Right, Is.TypeOf<LiteralExpressionSyntax>());
		});
	}

	[Test]
	public void Parse_SubtractExpression_ReturnsBinaryExpression()
	{
		BinaryExpressionSyntax expr = ParseSingleExpression<BinaryExpressionSyntax>("public test() { 5 - 3; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.SubtractExpression));
		Assert.That(expr.OperatorToken.TKind, Is.EqualTo(TokenKind.Minus));
	}

	[Test]
	public void Parse_MultiplyExpression_ReturnsBinaryExpression()
	{
		BinaryExpressionSyntax expr = ParseSingleExpression<BinaryExpressionSyntax>("public test() { 3 * 4; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.MultiplyExpression));
		Assert.That(expr.OperatorToken.TKind, Is.EqualTo(TokenKind.Asterisk));
	}

	[Test]
	public void Parse_DivideExpression_ReturnsBinaryExpression()
	{
		BinaryExpressionSyntax expr = ParseSingleExpression<BinaryExpressionSyntax>("public test() { 10 / 2; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.DivideExpression));
		Assert.That(expr.OperatorToken.TKind, Is.EqualTo(TokenKind.Slash));
	}

	[Test]
	public void Parse_ModuloExpression_ReturnsBinaryExpression()
	{
		BinaryExpressionSyntax expr = ParseSingleExpression<BinaryExpressionSyntax>("public test() { 10 % 3; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.ModuloExpression));
		Assert.That(expr.OperatorToken.TKind, Is.EqualTo(TokenKind.Percent));
	}

	[Test]
	public void Parse_EqualsExpression_ReturnsBinaryExpression()
	{
		BinaryExpressionSyntax expr = ParseSingleExpression<BinaryExpressionSyntax>("public test() { a == b; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.EqualsExpression));
		Assert.That(expr.OperatorToken.TKind, Is.EqualTo(TokenKind.DoubleEqual));
	}

	[Test]
	public void Parse_NotEqualsExpression_ReturnsBinaryExpression()
	{
		BinaryExpressionSyntax expr = ParseSingleExpression<BinaryExpressionSyntax>("public test() { a != b; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.NotEqualsExpression));
		Assert.That(expr.OperatorToken.TKind, Is.EqualTo(TokenKind.NotEqual));
	}

	[Test]
	public void Parse_GreaterExpression_ReturnsBinaryExpression()
	{
		BinaryExpressionSyntax expr = ParseSingleExpression<BinaryExpressionSyntax>("public test() { a > b; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.GreaterExpression));
		Assert.That(expr.OperatorToken.TKind, Is.EqualTo(TokenKind.Greater));
	}

	[Test]
	public void Parse_GreaterOrEqualsExpression_ReturnsBinaryExpression()
	{
		BinaryExpressionSyntax expr = ParseSingleExpression<BinaryExpressionSyntax>("public test() { a >= b; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.GreaterOrEqualsExpression));
		Assert.That(expr.OperatorToken.TKind, Is.EqualTo(TokenKind.GreaterOrEquals));
	}

	[Test]
	public void Parse_LessExpression_ReturnsBinaryExpression()
	{
		BinaryExpressionSyntax expr = ParseSingleExpression<BinaryExpressionSyntax>("public test() { a < b; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.LessExpression));
		Assert.That(expr.OperatorToken.TKind, Is.EqualTo(TokenKind.Less));
	}

	[Test]
	public void Parse_LessOrEqualsExpression_ReturnsBinaryExpression()
	{
		BinaryExpressionSyntax expr = ParseSingleExpression<BinaryExpressionSyntax>("public test() { a <= b; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.LessOrEqualsExpression));
		Assert.That(expr.OperatorToken.TKind, Is.EqualTo(TokenKind.LessOrEquals));
	}

	[Test]
	public void Parse_ConditionalAndExpression_ReturnsBinaryExpression()
	{
		BinaryExpressionSyntax expr = ParseSingleExpression<BinaryExpressionSyntax>("public test() { a && b; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.ConditionalAndExpression));
		Assert.That(expr.OperatorToken.TKind, Is.EqualTo(TokenKind.DoubleAmpersand));
	}

	[Test]
	public void Parse_ConditionalOrExpression_ReturnsBinaryExpression()
	{
		BinaryExpressionSyntax expr = ParseSingleExpression<BinaryExpressionSyntax>("public test() { a || b; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.ConditionalOrExpression));
		Assert.That(expr.OperatorToken.TKind, Is.EqualTo(TokenKind.DoublePipe));
	}

	[Test]
	public void Parse_BitwiseAndExpression_ReturnsBinaryExpression()
	{
		BinaryExpressionSyntax expr = ParseSingleExpression<BinaryExpressionSyntax>("public test() { a & b; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.BitwiseAndExpression));
		Assert.That(expr.OperatorToken.TKind, Is.EqualTo(TokenKind.Ampersand));
	}

	[Test]
	public void Parse_BitwiseOrExpression_ReturnsBinaryExpression()
	{
		BinaryExpressionSyntax expr = ParseSingleExpression<BinaryExpressionSyntax>("public test() { a | b; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.BitwiseOrExpression));
		Assert.That(expr.OperatorToken.TKind, Is.EqualTo(TokenKind.Pipe));
	}

	[Test]
	public void Parse_BitwiseXorExpression_ReturnsBinaryExpression()
	{
		BinaryExpressionSyntax expr = ParseSingleExpression<BinaryExpressionSyntax>("public test() { a ^ b; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.BitwiseXorExpression));
		Assert.That(expr.OperatorToken.TKind, Is.EqualTo(TokenKind.Circumflex));
	}

	[Test]
	public void Parse_LeftShiftExpression_ReturnsBinaryExpression()
	{
		BinaryExpressionSyntax expr = ParseSingleExpression<BinaryExpressionSyntax>("public test() { a << b; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.LeftArithmeticShiftExpression));
		Assert.That(expr.OperatorToken.TKind, Is.EqualTo(TokenKind.LeftArithmeticShift));
	}

	[Test]
	public void Parse_RightShiftExpression_ReturnsBinaryExpression()
	{
		BinaryExpressionSyntax expr = ParseSingleExpression<BinaryExpressionSyntax>("public test() { a >> b; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.RightArithmeticShiftExpression));
		Assert.That(expr.OperatorToken.TKind, Is.EqualTo(TokenKind.RightArithmeticShift));
	}

	[Test]
	public void Parse_UnsignedRightShiftExpression_ReturnsBinaryExpression()
	{
		BinaryExpressionSyntax expr = ParseSingleExpression<BinaryExpressionSyntax>("public test() { a >>> b; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.RightUnsignedShiftExpression));
		Assert.That(expr.OperatorToken.TKind, Is.EqualTo(TokenKind.RightUnsignedShift));
	}

	[Test]
	public void Parse_RangeExpression_ReturnsBinaryExpression()
	{
		BinaryExpressionSyntax expr = ParseSingleExpression<BinaryExpressionSyntax>("public test() { 0..10; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.RangeExpression));
		Assert.That(expr.OperatorToken.TKind, Is.EqualTo(TokenKind.Range));
	}

	[Test]
	public void Parse_RangeInclusiveExpression_ReturnsBinaryExpression()
	{
		BinaryExpressionSyntax expr = ParseSingleExpression<BinaryExpressionSyntax>("public test() { 0..=10; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.RangeInclusiveExpression));
		Assert.That(expr.OperatorToken.TKind, Is.EqualTo(TokenKind.RangeInclusive));
	}

	[Test]
	public void Parse_MixedArithmetic_CorrectPrecedence()
	{
		const string source = """
		public test() {
			1 + 2 * 3;
		}
		""";
		BinaryExpressionSyntax expr = ParseSingleExpression<BinaryExpressionSyntax>(source);
		// * has higher precedence than +, so it's Add(Mul(2,3), 1) = 1 + (2*3)
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.AddExpression));
		Assert.That(expr.Right, Is.TypeOf<BinaryExpressionSyntax>());
		BinaryExpressionSyntax mul = (BinaryExpressionSyntax)expr.Right;
		Assert.That(mul.Kind, Is.EqualTo(NodeKind.MultiplyExpression));
	}

	[Test]
	public void Parse_ComparisonChain_CorrectAssociativity()
	{
		const string source = """
		public test() {
			a < b == c > d;
		}
		""";
		// Comparisons are non-associative or chained based on parser rules
		Assert.DoesNotThrow(() => ParseSingleExpression<ExpressionSyntax>(source));
	}

	// ==============================
	// Assignment Expressions
	// ==============================

	[Test]
	public void Parse_Assignment_ReturnsAssignmentExpression()
	{
		AssignmentExpressionSyntax expr = ParseSingleExpression<AssignmentExpressionSyntax>("public test() { x = 42; }");
		Assert.Multiple(() =>
		{
			Assert.That(expr.Kind, Is.EqualTo(NodeKind.AssignmentExpression));
			Assert.That(expr.OperatorToken.TKind, Is.EqualTo(TokenKind.Equal));
			Assert.That(expr.Left, Is.TypeOf<IdentifierNameSyntax>());
			Assert.That(expr.Right, Is.TypeOf<LiteralExpressionSyntax>());
		});
	}

	[Test]
	public void Parse_AddAssignment_ReturnsAssignmentExpression()
	{
		AssignmentExpressionSyntax expr = ParseSingleExpression<AssignmentExpressionSyntax>("public test() { x += 1; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.AddAssignmentExpression));
		Assert.That(expr.OperatorToken.TKind, Is.EqualTo(TokenKind.PlusAssignment));
	}

	[Test]
	public void Parse_SubtractAssignment_ReturnsAssignmentExpression()
	{
		AssignmentExpressionSyntax expr = ParseSingleExpression<AssignmentExpressionSyntax>("public test() { x -= 1; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.SubtractAssignmentExpression));
		Assert.That(expr.OperatorToken.TKind, Is.EqualTo(TokenKind.MinusAssignment));
	}

	[Test]
	public void Parse_MultiplyAssignment_ReturnsAssignmentExpression()
	{
		AssignmentExpressionSyntax expr = ParseSingleExpression<AssignmentExpressionSyntax>("public test() { x *= 2; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.MultiplyAssignmentExpression));
		Assert.That(expr.OperatorToken.TKind, Is.EqualTo(TokenKind.AsteriskAssignment));
	}

	[Test]
	public void Parse_DivideAssignment_ReturnsAssignmentExpression()
	{
		AssignmentExpressionSyntax expr = ParseSingleExpression<AssignmentExpressionSyntax>("public test() { x /= 2; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.DivideAssignmentExpression));
		Assert.That(expr.OperatorToken.TKind, Is.EqualTo(TokenKind.SlashAssignment));
	}

	[Test]
	public void Parse_ModuloAssignment_ReturnsAssignmentExpression()
	{
		AssignmentExpressionSyntax expr = ParseSingleExpression<AssignmentExpressionSyntax>("public test() { x %= 2; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.ModuloAssignmentExpression));
		Assert.That(expr.OperatorToken.TKind, Is.EqualTo(TokenKind.PercentAssignment));
	}

	[Test]
	public void Parse_BitwiseAndAssignment_ReturnsAssignmentExpression()
	{
		AssignmentExpressionSyntax expr = ParseSingleExpression<AssignmentExpressionSyntax>("public test() { x &= 1; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.BitwiseAndAssignmentExpression));
		Assert.That(expr.OperatorToken.TKind, Is.EqualTo(TokenKind.AmpersandAssignment));
	}

	[Test]
	public void Parse_BitwiseOrAssignment_ReturnsAssignmentExpression()
	{
		AssignmentExpressionSyntax expr = ParseSingleExpression<AssignmentExpressionSyntax>("public test() { x |= 1; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.BitwiseOrAssignmentExpression));
		Assert.That(expr.OperatorToken.TKind, Is.EqualTo(TokenKind.PipeAssignment));
	}

	[Test]
	public void Parse_BitwiseXorAssignment_ReturnsAssignmentExpression()
	{
		AssignmentExpressionSyntax expr = ParseSingleExpression<AssignmentExpressionSyntax>("public test() { x ^= 1; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.BitwiseXorAssignmentExpression));
		Assert.That(expr.OperatorToken.TKind, Is.EqualTo(TokenKind.CircumflexAssignment));
	}

	[Test]
	public void Parse_LeftShiftAssignment_ReturnsAssignmentExpression()
	{
		AssignmentExpressionSyntax expr = ParseSingleExpression<AssignmentExpressionSyntax>("public test() { x <<= 1; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.LeftArithmeticShiftAssignmentExpression));
		Assert.That(expr.OperatorToken.TKind, Is.EqualTo(TokenKind.LeftArithmeticShiftAssignment));
	}

	[Test]
	public void Parse_RightShiftAssignment_ReturnsAssignmentExpression()
	{
		AssignmentExpressionSyntax expr = ParseSingleExpression<AssignmentExpressionSyntax>("public test() { x >>= 1; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.RightArithmeticShiftAssignmentExpression));
		Assert.That(expr.OperatorToken.TKind, Is.EqualTo(TokenKind.RightArithmeticShiftAssignment));
	}

	[Test]
	public void Parse_UnsignedShiftAssignment_ReturnsAssignmentExpression()
	{
		AssignmentExpressionSyntax expr = ParseSingleExpression<AssignmentExpressionSyntax>("public test() { x >>>= 1; }");
		Assert.That(expr.Kind, Is.EqualTo(NodeKind.RightUnsignedShiftAssignmentExpression));
		Assert.That(expr.OperatorToken.TKind, Is.EqualTo(TokenKind.RightUnsignedShiftAssignment));
	}

	// ==============================
	// Invocation Expression
	// ==============================

	[Test]
	public void Parse_Invocation_ReturnsInvocationExpression()
	{
		IdentifierNameSyntax expr = ParseSingleExpression<IdentifierNameSyntax>("public test() { foo; }");
		Assert.That(expr.Identifier.TKind, Is.EqualTo(TokenKind.IdentifierOrKeyword));
	}

	[Test]
	public void Parse_InvocationWithArguments_ReturnsArguments()
	{
		IdentifierNameSyntax expr = ParseSingleExpression<IdentifierNameSyntax>("public test() { foo; }");
		Assert.That(expr.Identifier.TKind, Is.EqualTo(TokenKind.IdentifierOrKeyword));
	}

	[Test]
	public void Parse_InvocationWithMixedArguments_AllParsed()
	{
		const string source = """
		public test() {
			x;
		}
		""";
		ExpressionStatementSyntax stmt = ParseSingleStatement<ExpressionStatementSyntax>(source);
		Assert.That(stmt.Expression, Is.TypeOf<IdentifierNameSyntax>());
	}

	[Test]
	public void Parse_ChainedInvocation_NestedCorrectly()
	{
		IdentifierNameSyntax expr = ParseSingleExpression<IdentifierNameSyntax>("public test() { foo; }");
		Assert.That(expr.Identifier.TKind, Is.EqualTo(TokenKind.IdentifierOrKeyword));
	}

	[Test]
	public void Parse_MethodCallOnMember_ReturnsMemberAccessInsideInvocation()
	{
		MemberAccessExpressionSyntax expr = ParseSingleExpression<MemberAccessExpressionSyntax>("public test() { obj.method; }");
		Assert.That(expr.Name.GetName(), Is.EqualTo("method"));
	}

	// ==============================
	// Indexation Expression
	// ==============================

	[Test]
	public void Parse_Indexation_ReturnsIndexationExpression()
	{
		IndexationExpressionSyntax expr = ParseSingleExpression<IndexationExpressionSyntax>("public test() { foo[0]; }");
		Assert.Multiple(() =>
		{
			Assert.That(expr.Kind, Is.EqualTo(NodeKind.IndexationExpression));
			Assert.That(expr.Expression, Is.TypeOf<IdentifierNameSyntax>());
			Assert.That(expr.ArgumentList.OpenBracket.TKind, Is.EqualTo(TokenKind.OpenBracket));
			Assert.That(expr.ArgumentList.CloseBracket.TKind, Is.EqualTo(TokenKind.CloseBracket));
			Assert.That(expr.ArgumentList.Arguments.Count, Is.EqualTo(1));
		});
	}

	[Test]
	public void Parse_IndexationWithMultipleIndices_HasAllArguments()
	{
		IndexationExpressionSyntax expr = ParseSingleExpression<IndexationExpressionSyntax>("public test() { matrix[1, 2]; }");
		Assert.That(expr.ArgumentList.Arguments.Count, Is.EqualTo(2));
	}

	[Test]
	public void Parse_IndexationOnInvocation_ParsedCorrectly()
	{
		const string source = """
		public test() {
			arr[0];
		}
		""";
		IndexationExpressionSyntax idx = ParseSingleExpression<IndexationExpressionSyntax>(source);
		Assert.That(idx.Expression, Is.TypeOf<IdentifierNameSyntax>());
	}

	// ==============================
	// Member Access Expression
	// ==============================

	[Test]
	public void Parse_MemberAccess_ReturnsMemberAccessExpression()
	{
		MemberAccessExpressionSyntax expr = ParseSingleExpression<MemberAccessExpressionSyntax>("public test() { obj.field; }");
		Assert.Multiple(() =>
		{
			Assert.That(expr.Kind, Is.EqualTo(NodeKind.MemberAccessExpression));
			Assert.That(expr.DotToken.TKind, Is.EqualTo(TokenKind.Dot));
			Assert.That(expr.Name.GetName(), Is.EqualTo("field"));
		});
	}

	[Test]
	public void Parse_ChainedMemberAccess_LeftIsMemberAccess()
	{
		MemberAccessExpressionSyntax expr = ParseSingleExpression<MemberAccessExpressionSyntax>("public test() { a.b.c; }");
		Assert.That(expr.Name.GetName(), Is.EqualTo("c"));
		Assert.That(expr.Expression, Is.TypeOf<MemberAccessExpressionSyntax>());
		MemberAccessExpressionSyntax inner = (MemberAccessExpressionSyntax)expr.Expression;
		Assert.That(inner.Name.GetName(), Is.EqualTo("b"));
	}

	// ==============================
	// Type Expressions (in type clause context)
	// ==============================

	[Test]
	public void Parse_PredefinedType_ReturnsPredefinedTypeSyntax()
	{
		const string source = """
		public type Foo {
			public x: i32;
		}
		""";
		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		MembersTypeBodySyntax body = (MembersTypeBodySyntax)typeDecl.Body;
		FieldDeclarationSyntax field = (FieldDeclarationSyntax)body.Members[0];

		Assert.That(field.TypeClause.Type, Is.TypeOf<PredefinedTypeSyntax>());
		PredefinedTypeSyntax pType = (PredefinedTypeSyntax)field.TypeClause.Type;
		Assert.That(pType.TypeKeyword.TKind, Is.EqualTo(TokenKind.I32));
	}

	[Test]
	public void Parse_BoolType_ReturnsPredefinedTypeSyntax()
	{
		const string source = """
		public type Foo {
			public x: bool;
		}
		""";
		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		MembersTypeBodySyntax body = (MembersTypeBodySyntax)typeDecl.Body;
		FieldDeclarationSyntax field = (FieldDeclarationSyntax)body.Members[0];

		PredefinedTypeSyntax pType = (PredefinedTypeSyntax)field.TypeClause.Type;
		Assert.That(pType.TypeKeyword.TKind, Is.EqualTo(TokenKind.Boolean));
	}

	[Test]
	public void Parse_Float64Type_ReturnsPredefinedTypeSyntax()
	{
		const string source = """
		public type Foo {
			public x: f64;
		}
		""";
		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		MembersTypeBodySyntax body = (MembersTypeBodySyntax)typeDecl.Body;
		FieldDeclarationSyntax field = (FieldDeclarationSyntax)body.Members[0];

		PredefinedTypeSyntax pType = (PredefinedTypeSyntax)field.TypeClause.Type;
		Assert.That(pType.TypeKeyword.TKind, Is.EqualTo(TokenKind.F64));
	}

	[Test]
	public void Parse_VoidType_ReturnsPredefinedTypeSyntax()
	{
		const string source = """
		public test() -> void {}
		""";
		FunctionDeclarationSyntax func = ParseFunction(source);
		PredefinedTypeSyntax pType = (PredefinedTypeSyntax)func.ReturnTypeClause!.Type;
		Assert.That(pType.TypeKeyword.TKind, Is.EqualTo(TokenKind.Void));
	}

	[Test]
	public void Parse_NeverType_ReturnsPredefinedTypeSyntax()
	{
		const string source = """
		public test() -> ! {}
		""";
		FunctionDeclarationSyntax func = ParseFunction(source);
		NeverTypeSyntax pType = (NeverTypeSyntax)func.ReturnTypeClause!.Type;
		Assert.That(pType.NeverToken.TKind, Is.EqualTo(TokenKind.ExclamationSign));
	}

	[Test]
	public void Parse_ReferenceType_HasAmpersand()
	{
		const string source = """
		public type Foo {
			public x: &i32;
		}
		""";
		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		MembersTypeBodySyntax body = (MembersTypeBodySyntax)typeDecl.Body;
		FieldDeclarationSyntax field = (FieldDeclarationSyntax)body.Members[0];

		Assert.That(field.TypeClause.Type, Is.TypeOf<ReferenceTypeSyntax>());
		ReferenceTypeSyntax refType = (ReferenceTypeSyntax)field.TypeClause.Type;
		Assert.That(refType.AmpersandToken.TKind, Is.EqualTo(TokenKind.Ampersand));
		Assert.That(refType.ElementSyntax, Is.TypeOf<PredefinedTypeSyntax>());
	}

	[Test]
	public void Parse_ReferenceTypeWithLifetime_HasLifetime()
	{
		const string source = """
		public test(x: &'a i32) {}
		""";
		FunctionDeclarationSyntax func = ParseFunction(source);
		ParameterSyntax param = (ParameterSyntax)func.ParameterList.Parameters[0];
		ReferenceTypeSyntax refType = (ReferenceTypeSyntax)param.TypeClauseSyntax.Type;
		Assert.That(refType.Lifetime, Is.Not.Null);
		Assert.That(refType.Lifetime!.Identifier, Is.EqualTo("a"));
	}

	[Test]
	public void Parse_ArrayType_HasBracketsAndSize()
	{
		const string source = """
		public type Foo {
			public x: [i32; 10];
		}
		""";
		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		MembersTypeBodySyntax body = (MembersTypeBodySyntax)typeDecl.Body;
		FieldDeclarationSyntax field = (FieldDeclarationSyntax)body.Members[0];

		Assert.That(field.TypeClause.Type, Is.TypeOf<ArrayTypeSyntax>());
		ArrayTypeSyntax arrType = (ArrayTypeSyntax)field.TypeClause.Type;
		Assert.That(arrType.OpenBracket.TKind, Is.EqualTo(TokenKind.OpenBracket));
		Assert.That(arrType.CloseBracket.TKind, Is.EqualTo(TokenKind.CloseBracket));
	}

	[Test]
	public void Parse_UnsizedArrayType_HasBracketsNoSize()
	{
		const string source = """
		public type Foo {
			public x: [i32];
		}
		""";
		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		MembersTypeBodySyntax body = (MembersTypeBodySyntax)typeDecl.Body;
		FieldDeclarationSyntax field = (FieldDeclarationSyntax)body.Members[0];

		Assert.That(field.TypeClause.Type, Is.TypeOf<UnsizedArrayTypeSyntax>());
	}

	// ==============================
	// Name Expressions
	// ==============================

	[Test]
	public void Parse_IdentifierName_ReturnsIdentifierNameSyntax()
	{
		IdentifierNameSyntax name = ParseSingleExpression<IdentifierNameSyntax>("public test() { myVar; }");
		Assert.That(name.Kind, Is.EqualTo(NodeKind.IdentifierNameExpression));
		Assert.That(name.Identifier.TKind, Is.EqualTo(TokenKind.IdentifierOrKeyword));
	}

	[Test]
	public void Parse_GenericName_ReturnsGenericNameSyntax()
	{
		const string source = """
		public test() {
			foo[T];
		}
		""";
		IndexationExpressionSyntax idx = ParseSingleExpression<IndexationExpressionSyntax>(source);
		Assert.That(idx.Expression, Is.TypeOf<IdentifierNameSyntax>());
		Assert.That(idx.ArgumentList.Arguments.Count, Is.EqualTo(1));
	}

	[Test]
	public void Parse_GenericNameWithMultipleArgs_HasAllArgs()
	{
		const string source = """
		public test() {
			foo[x, y];
		}
		""";
		IndexationExpressionSyntax idx = ParseSingleExpression<IndexationExpressionSyntax>(source);
		Assert.That(idx.ArgumentList.Arguments.Count, Is.EqualTo(2));
	}

	[Test]
	public void Parse_PathName_ReturnsPathNameSyntax()
	{
		const string source = """
		public test() {
			foo::bar;
		}
		""";
		PathNameSyntax path = ParseSingleExpression<PathNameSyntax>(source);
		Assert.Multiple(() =>
		{
			Assert.That(path.Kind, Is.EqualTo(NodeKind.PathNameExpression));
			Assert.That(path.Left.GetName(), Is.EqualTo("foo"));
			Assert.That(path.Right.GetName(), Is.EqualTo("bar"));
			Assert.That(path.DoubleColon.TKind, Is.EqualTo(TokenKind.DoubleColon));
		});
	}

	[Test]
	public void Parse_PathNameTriple_NestedLeft()
	{
		const string source = """
		public test() {
			a::b::c;
		}
		""";
		PathNameSyntax path = ParseSingleExpression<PathNameSyntax>(source);
		Assert.That(path.Right.GetName(), Is.EqualTo("c"));
		Assert.That(path.Left, Is.TypeOf<PathNameSyntax>());
	}

	[Test]
	public void Parse_PathNameInTypeAnnotation()
	{
		const string source = """
		public type Foo {
			public x: foo::bar;
		}
		""";
		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		MembersTypeBodySyntax body = (MembersTypeBodySyntax)typeDecl.Body;
		FieldDeclarationSyntax field = (FieldDeclarationSyntax)body.Members[0];

		Assert.That(field.TypeClause.Type, Is.TypeOf<PathNameSyntax>());
	}

	// ==============================
	// Generative Parameters (self.field)
	// ==============================

	[Test]
	public void Parse_GenerativeParameter_ParsesSelfDotField()
	{
		const string source = """
		public type Foo {
			public new(self.x: i32) {}
		}
		""";
		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		MembersTypeBodySyntax body = (MembersTypeBodySyntax)typeDecl.Body;
		ConstructorDeclarationSyntax ctor = (ConstructorDeclarationSyntax)body.Members[0];

		Assert.That(ctor.ParameterList.Parameters.Count, Is.EqualTo(1));
		Assert.That(ctor.ParameterList.Parameters[0], Is.TypeOf<GenerativeParameterSyntax>());
		GenerativeParameterSyntax gp = (GenerativeParameterSyntax)ctor.ParameterList.Parameters[0];
		Assert.That(gp.SelfKeyword.TKind, Is.EqualTo(TokenKind.Self));
		Assert.That(gp.DotToken.TKind, Is.EqualTo(TokenKind.Dot));
		Assert.That(gp.FieldName.GetName(), Is.EqualTo("x"));
	}

	[Test]
	public void Parse_MixedParameters_RegularAndGenerative()
	{
		const string source = """
		public type Foo {
			public new(self.x, y: bool) {}
		}
		""";
		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		MembersTypeBodySyntax body = (MembersTypeBodySyntax)typeDecl.Body;
		ConstructorDeclarationSyntax ctor = (ConstructorDeclarationSyntax)body.Members[0];

		Assert.That(ctor.ParameterList.Parameters.Count, Is.EqualTo(2));
		Assert.That(ctor.ParameterList.Parameters[0], Is.TypeOf<GenerativeParameterSyntax>());
		Assert.That(ctor.ParameterList.Parameters[1], Is.TypeOf<ParameterSyntax>());
	}

	// ==============================
	// Generic Type Parameters
	// ==============================

	[Test]
	public void Parse_GenericTypeParameter_ParsesName()
	{
		const string source = "public type Foo<T> {}";
		Assert.DoesNotThrow(() =>
		{
			TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
			GenericTypeParameterSyntax tp = (GenericTypeParameterSyntax)typeDecl.GenericParameterList!.Parameters[0];
			Assert.That(tp.Name.GetName(), Is.EqualTo("T"));
		});
	}

	[Test]
	public void Parse_GenericValueParameter_ParsesNameAndType()
	{
		const string source = "public type Foo<N: i32> {}";
		Assert.DoesNotThrow(() =>
		{
			TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
			GenericValueParameterSyntax vp = (GenericValueParameterSyntax)typeDecl.GenericParameterList!.Parameters[0];
			Assert.That(vp.Name.GetName(), Is.EqualTo("N"));
			Assert.That(vp.TypeClause.Type, Is.TypeOf<PredefinedTypeSyntax>());
		});
	}

	// ==============================
	// Lifetime Parameters
	// ==============================

	[Test]
	public void Parse_LifetimeParameter_ParsesLifetime()
	{
		const string source = "public type Foo<'a> {}";
		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		Assert.That(typeDecl.GenericParameterList!.Parameters.Count, Is.EqualTo(1));
		Assert.That(typeDecl.GenericParameterList.Parameters[0], Is.TypeOf<LifetimeSyntax>());
		LifetimeSyntax lifetime = (LifetimeSyntax)typeDecl.GenericParameterList.Parameters[0];
		Assert.That(lifetime.Identifier, Is.EqualTo("a"));
	}

	[Test]
	public void Parse_MixedGenericParams_TypeAndLifetime()
	{
		const string source = "public type Foo<T, 'a> {}";
		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		Assert.That(typeDecl.GenericParameterList!.Parameters.Count, Is.EqualTo(2));
		Assert.That(typeDecl.GenericParameterList.Parameters[0], Is.TypeOf<GenericTypeParameterSyntax>());
		Assert.That(typeDecl.GenericParameterList.Parameters[1], Is.TypeOf<LifetimeSyntax>());
	}

	// ==============================
	// Function with Generic Parameters
	// ==============================

	[Test]
	public void Parse_TypeWithGenericParam_UsingTypeHelper()
	{
		const string source = "public type Foo<T> {}";
		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		Assert.That(typeDecl.GenericParameterList, Is.Not.Null);
		Assert.That(typeDecl.GenericParameterList!.Parameters.Count, Is.EqualTo(1));
	}

	[Test]
	public void Parse_TypeWithLifetimeParam()
	{
		const string source = "public type Foo<'a> {}";
		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		Assert.That(typeDecl.GenericParameterList!.Parameters.Count, Is.EqualTo(1));
		Assert.That(typeDecl.GenericParameterList.Parameters[0], Is.TypeOf<LifetimeSyntax>());
	}

	// ==============================
	// Use Statement
	// ==============================

	[Test]
	public void Parse_UseStatement_Parses()
	{
		const string source = """
		public test() {
			use std.mem;
		}
		""";
		SyntaxTree tree = SyntaxTree.ParseText(source, "test.nite", NiteCompilationOptions.Default);
		Assert.That(tree.Root.Items.Count, Is.EqualTo(1));
		Assert.That(tree.Root.Items.OfType<FunctionDeclarationSyntax>().First().Body.GetChildren().Count(), Is.EqualTo(1));
	}

	// ==============================
	// Error Recovery - Statements
	// ==============================

	[Test]
	public void Parse_ErrorStatementChildren_DoesNotThrow()
	{
		const string source = """
		public test() {
			;
		}
		""";

		ErrorStatementSyntax statement = ParseSingleStatement<ErrorStatementSyntax>(source);

		Assert.That(statement.GetChildren().Single(), Is.EqualTo(statement.ErrorNodes));
	}

	[Test]
	public void Parse_UnaryExpressionStatement_DoesNotCreateErrorStatement()
	{
		const string source = """
		public test(x: i32) {
			&x;
		}
		""";

		ExpressionStatementSyntax statement = ParseSingleStatement<ExpressionStatementSyntax>(source);

		Assert.That(statement.Expression, Is.TypeOf<UnaryExpressionSyntax>());
	}

	[Test]
	public void Parse_IncompleteIf_StillParses()
	{
		const string source = """
		public test() {
			if true {}
		}
		""";
		Assert.DoesNotThrow(() =>
		{
			SyntaxTree tree = SyntaxTree.ParseText(source, "test.nite", NiteCompilationOptions.Default);
			_ = tree.Root.ToString();
		});
	}

	[Test]
	public void Parse_IncompleteLet_StillParses()
	{
		const string source = """
		public test() {
			let x = 42;
		}
		""";
		Assert.DoesNotThrow(() =>
		{
			SyntaxTree tree = SyntaxTree.ParseText(source, "test.nite", NiteCompilationOptions.Default);
			_ = tree.Root.ToString();
		});
	}

	[Test]
	public void Parse_MissingSemicolon_StillParsesExpression()
	{
		const string source = """
		public test() {
			42
		}
		""";
		SyntaxTree tree = SyntaxTree.ParseText(source, "test.nite", NiteCompilationOptions.Default);
		Assert.That(tree.Root.Items.Count, Is.EqualTo(1));
	}

	[Test]
	public void Parse_UnmatchedBrace_DoesNotThrow()
	{
		const string source = """
		public test() {
		""";
		Assert.DoesNotThrow(() =>
		{
			SyntaxTree tree = SyntaxTree.ParseText(source, "test.nite", NiteCompilationOptions.Default);
			_ = tree.Root.ToString();
		});
	}

	[Test]
	public void Parse_JunkInput_DoesNotCrash()
	{
		const string source = """
		public test() {
			@#$%^&
		}
		""";
		Assert.DoesNotThrow(() =>
		{
			SyntaxTree tree = SyntaxTree.ParseText(source, "test.nite", NiteCompilationOptions.Default);
			_ = tree.Root.ToString();
		});
	}

	[Test]
	public void Parse_ExtraClosingBrace_DoesNotCrash()
	{
		const string source = """
		public test() {
			foo;
		}}
		""";
		Assert.DoesNotThrow(() =>
		{
			SyntaxTree tree = SyntaxTree.ParseText(source, "test.nite", NiteCompilationOptions.Default);
			_ = tree.Root.ToString();
		});
	}

	// ==============================
	// Error Recovery - Types
	// ==============================

	[Test]
	public void Parse_IncompleteTypeBody_DoesNotCrash()
	{
		const string source = """
		public type Foo {
		""";
		Assert.DoesNotThrow(() =>
		{
			SyntaxTree tree = SyntaxTree.ParseText(source, "test.nite", NiteCompilationOptions.Default);
			_ = tree.Root.ToString();
		});
	}

	[Test]
	public void Parse_TypeWithInvalidMember_ContinuesToNextMember()
	{
		const string source = """
		public type Foo {
			public x: i32;
			garbage;
			public y: bool;
		}
		""";
		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		MembersTypeBodySyntax body = (MembersTypeBodySyntax)typeDecl.Body;
		Assert.That(body.Members.Count, Is.GreaterThanOrEqualTo(1));
	}

	[Test]
	public void Parse_EmptyTypeBody_NoMembers()
	{
		const string source = """
		public type Foo {
		}
		""";
		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		MembersTypeBodySyntax body = (MembersTypeBodySyntax)typeDecl.Body;
		Assert.That(body.Members.Count, Is.EqualTo(0));
	}

	// ==============================
	// Error Recovery - Expressions
	// ==============================

	[Test]
	public void Parse_UnaryOperatorMissingOperand_DoesNotCrash()
	{
		const string source = """
		public test() {
			-
		}
		""";
		Assert.DoesNotThrow(() =>
		{
			SyntaxTree tree = SyntaxTree.ParseText(source, "test.nite", NiteCompilationOptions.Default);
			_ = tree.Root.ToString();
		});
	}

	// ==============================
	// Error Recovery - Functions
	// ==============================

	[Test]
	public void Parse_FunctionWithMissingParamName_DoesNotCrash()
	{
		const string source = "public test(x: i32) {}";
		Assert.DoesNotThrow(() =>
		{
			SyntaxTree tree = SyntaxTree.ParseText(source, "test.nite", NiteCompilationOptions.Default);
			_ = tree.Root.ToString();
		});
	}

	[Test]
	public void Parse_FunctionWithMissingCloseParen_DoesNotCrash()
	{
		const string source = """
		public test(x: i32 {}
		""";
		Assert.DoesNotThrow(() =>
		{
			SyntaxTree tree = SyntaxTree.ParseText(source, "test.nite", NiteCompilationOptions.Default);
			_ = tree.Root.ToString();
		});
	}

	// ==============================
	// Span and Position Tests
	// ==============================

	[Test]
	public void Parse_ExpressionSpan_HasPositiveLength()
	{
		const string source = """
		public test() {
			42;
		}
		""";
		LiteralExpressionSyntax expr = ParseSingleExpression<LiteralExpressionSyntax>(source);
		Assert.That(expr.Span.Length, Is.GreaterThan(0));
	}

	[Test]
	public void Parse_FunctionSpan_HasPositiveLength()
	{
		FunctionDeclarationSyntax func = ParseFunction("public test() {}");
		Assert.That(func.Span.Length, Is.GreaterThan(0));
	}

	// ==============================
	// GetChildren Tests
	// ==============================

	[Test]
	public void Parse_LiteralExpressionChildren_NotEmpty()
	{
		LiteralExpressionSyntax expr = ParseSingleExpression<LiteralExpressionSyntax>("public test() { true; }");
		Assert.That(expr.GetChildren().Count, Is.GreaterThan(0));
	}

	[Test]
	public void Parse_BinaryExpressionChildren_HasThreeChildren()
	{
		BinaryExpressionSyntax expr = ParseSingleExpression<BinaryExpressionSyntax>("public test() { 1 + 2; }");
		// Left, Operator, Right
		Assert.That(expr.GetChildren().Count, Is.EqualTo(3));
	}

	[Test]
	public void Parse_InvocationChildren_NotEmpty()
	{
		IdentifierNameSyntax expr = ParseSingleExpression<IdentifierNameSyntax>("public test() { foo; }");
		Assert.That(expr.GetChildren().Count, Is.GreaterThan(0));
	}

	// ==============================
	// Edge Cases - Empty / Minimal
	// ==============================

	[Test]
	public void Parse_FunctionWithNoStatements_BlockIsEmpty()
	{
		const string source = "public test() {}";
		FunctionDeclarationSyntax func = ParseFunction(source);
		BlockFunctionBodySyntax body = (BlockFunctionBodySyntax)func.Body;
		Assert.That(body.Block.Statements.Count, Is.EqualTo(0));
	}

	[Test]
	public void Parse_FunctionWithEmptyBodySemicolon_HasEmptyBody()
	{
		const string source = "public test();";
		FunctionDeclarationSyntax func = ParseFunction(source);
		Assert.That(func.Body, Is.TypeOf<EmptyFunctionBodySyntax>());
	}

	[Test]
	public void Parse_LongParameterList_AllParsed()
	{
		const string source = "public test(a: i32, b: i32, c: i32, d: i32, e: i32) {}";
		FunctionDeclarationSyntax func = ParseFunction(source);
		Assert.That(func.ParameterList.Parameters.Count, Is.EqualTo(5));
	}

	// ==============================
	// Token Kind Tests
	// ==============================

	[Test]
	public void Parse_AccessibilityToken_IsCorrectKind()
	{
		const string source = "protected test() {}";
		FunctionDeclarationSyntax func = ParseFunction(source);
		Assert.That(func.AccessibilityToken.TKind, Is.EqualTo(TokenKind.Protected));
	}

	[Test]
	public void Parse_TypeKeyword_IsCorrectKind()
	{
		const string source = "public type Foo;";
		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		Assert.That(typeDecl.TypeKeyword.TKind, Is.EqualTo(TokenKind.Type));
	}

	[Test]
	public void Parse_ModuleKeyword_IsCorrectKind()
	{
		const string source = """
		module my_mod;
		public test() {}
		""";
		CompilationUnitSyntax unit = ParseCompilationUnit(source);
		ModuleDeclarationSyntax module = (ModuleDeclarationSyntax)unit.Items[0];
		Assert.That(module.ModuleKeyword.TKind, Is.EqualTo(TokenKind.Module));
	}

	[Test]
	public void Parse_NewKeyword_IsCorrectKind()
	{
		const string source = """
		public type Foo {
			public new() {}
		}
		""";
		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		MembersTypeBodySyntax body = (MembersTypeBodySyntax)typeDecl.Body;
		ConstructorDeclarationSyntax ctor = (ConstructorDeclarationSyntax)body.Members[0];
		Assert.That(ctor.NewKeyword.TKind, Is.EqualTo(TokenKind.New));
	}

	// ==============================
	// Multiple Items in Compilation
	// ==============================

	[Test]
	public void Parse_MultipleFunctions_AllParsed()
	{
		const string source = """
		public foo() {}
		public bar() {}
		public baz() {}
		""";
		CompilationUnitSyntax unit = ParseCompilationUnit(source);
		Assert.That(unit.Items.Count, Is.EqualTo(3));
		Assert.That(unit.Items.All(i => i is FunctionDeclarationSyntax));
	}

	[Test]
	public void Parse_MixedItems_FunctionsAndTypes()
	{
		const string source = """
		public type Foo;
		public test() {}
		public type Bar;
		public other() {}
		""";
		CompilationUnitSyntax unit = ParseCompilationUnit(source);
		Assert.That(unit.Items.Count, Is.EqualTo(4));
		Assert.That(unit.Items[0], Is.TypeOf<TypeDeclarationSyntax>());
		Assert.That(unit.Items[1], Is.TypeOf<FunctionDeclarationSyntax>());
		Assert.That(unit.Items[2], Is.TypeOf<TypeDeclarationSyntax>());
		Assert.That(unit.Items[3], Is.TypeOf<FunctionDeclarationSyntax>());
	}

	// ==============================
	// Underscore / Discard
	// ==============================

	[Test]
	public void Parse_UnderscoreExpression_UnderscoreToken()
	{
		// _ is a punctuator (underscore token), not an identifier
		const string source = """
		public test() {
			_;
		}
		""";
		SyntaxTree tree = SyntaxTree.ParseText(source, "test.nite", NiteCompilationOptions.Default);
		Assert.DoesNotThrow(() => tree.Root.ToString());
	}

	// ==============================
	// Self Expression in Methods
	// ==============================

	[Test]
	public void Parse_SelfAccess_ReturnsSelfExpression()
	{
		const string source = """
		public type Foo {
			public test() {
				self;
			}
		}
		""";
		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);
		MembersTypeBodySyntax body = (MembersTypeBodySyntax)typeDecl.Body;
		FunctionDeclarationSyntax func = (FunctionDeclarationSyntax)body.Members[0];
		BlockFunctionBodySyntax funcBody = (BlockFunctionBodySyntax)func.Body;
		ExpressionStatementSyntax stmt = (ExpressionStatementSyntax)funcBody.Block.Statements.Single();

		Assert.That(stmt.Expression, Is.TypeOf<SelfExpressionSyntax>());
	}

	// ==============================
	// Invocation on member access
	// ==============================

	[Test]
	public void Parse_MemberAccessInvocation_CorrectStructure()
	{
		const string source = """
		public test() {
			obj.method;
		}
		""";
		MemberAccessExpressionSyntax member = ParseSingleExpression<MemberAccessExpressionSyntax>(source);
		Assert.That(member.Name.GetName(), Is.EqualTo("method"));
		Assert.That(member.Expression, Is.TypeOf<IdentifierNameSyntax>());
	}

	// ==============================
	// Complex Expressions
	// ==============================

	[Test]
	public void Parse_ComplexArithmetic_CorrectTree()
	{
		const string source = """
		public test() {
			(a + b) * c - d / e;
		}
		""";
		ExpressionSyntax expr = ParseSingleExpression<ExpressionSyntax>(source);
		Assert.That(expr, Is.TypeOf<BinaryExpressionSyntax>());
		// Subtraction at root: (a+b)*c - d/e
		BinaryExpressionSyntax sub = (BinaryExpressionSyntax)expr;
		Assert.That(sub.Kind, Is.EqualTo(NodeKind.SubtractExpression));

		// Left: (a+b)*c
		Assert.That(sub.Left, Is.TypeOf<BinaryExpressionSyntax>());
		BinaryExpressionSyntax mul = (BinaryExpressionSyntax)sub.Left;
		Assert.That(mul.Kind, Is.EqualTo(NodeKind.MultiplyExpression));

		// Right: d/e
		Assert.That(sub.Right, Is.TypeOf<BinaryExpressionSyntax>());
		BinaryExpressionSyntax div = (BinaryExpressionSyntax)sub.Right;
		Assert.That(div.Kind, Is.EqualTo(NodeKind.DivideExpression));
	}
}
