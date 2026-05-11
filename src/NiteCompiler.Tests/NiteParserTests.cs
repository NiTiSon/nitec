using System.Linq;
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
