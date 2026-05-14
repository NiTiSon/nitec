using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Compilation;

namespace NiteCompiler.Tests;

[TestFixture]
public class NiteParserTests
{
	[Test]
	public void Parse_IndexationExpression_UsesBracketedArgumentList()
	{
		const string source = """
		public test() {
			foo[1, 2];
		}
		""";

		ExpressionStatementSyntax statement = ParseSingleStatement<ExpressionStatementSyntax>(source);

		Assert.Multiple(() =>
		{
			Assert.That(statement.Expression, Is.TypeOf<IndexationExpressionSyntax>());
			IndexationExpressionSyntax indexation = (IndexationExpressionSyntax)statement.Expression;
			Assert.That(indexation.Kind, Is.EqualTo(NodeKind.IndexationExpression));
			Assert.That(indexation.ArgumentList.OpenBracket.TKind, Is.EqualTo(TokenKind.OpenBracket));
			Assert.That(indexation.ArgumentList.CloseBracket.TKind, Is.EqualTo(TokenKind.CloseBracket));
			Assert.That(indexation.ArgumentList.Arguments.Count, Is.EqualTo(2));
		});
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
			Assert.That(field.TypeClause.Type.Kind, Is.EqualTo(NodeKind.PredefinedType));
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
			FunctionDeclarationSyntax func = (FunctionDeclarationSyntax)body.Members[0];
			Assert.That(func.Name.GetName(), Is.EqualTo("test"));
			Assert.That(func.AccessibilityToken.TKind, Is.EqualTo(TokenKind.Public));
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
	public void Parse_TypeBodyWithMixedMembers_ParsesAllKinds()
	{
		const string source = """
		public type Foo {
			public x: i32;
			public test() {}
			public type Bar {
				public y: bool;
			}
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
			FunctionDeclarationSyntax func = (FunctionDeclarationSyntax)body.Members[0];
			Assert.That(func.Name.GetName(), Is.EqualTo("test"));
			Assert.That(func.AccessibilityToken.TKind, Is.EqualTo(TokenKind.None));
		});
	}

	[Test]
	public void Parse_TypeWithEmptyBody_ParsesSemicolon()
	{
		const string source = """
		public type Foo;
		""";

		TypeDeclarationSyntax typeDecl = ParseTypeDeclaration(source);

		Assert.That(typeDecl.Body, Is.TypeOf<EmptyTypeBodySyntax>());
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

	private static TypeDeclarationSyntax ParseTypeDeclaration(string source)
	{
		SyntaxTree tree = SyntaxTree.ParseText(source, "test.nite", NiteCompilationOptions.Default);
		return tree.Root.Items.OfType<TypeDeclarationSyntax>().Single();
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
}
