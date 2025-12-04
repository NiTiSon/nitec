using System;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Compilation;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class StatementBinder : ScopedBinder
{
	public StatementBinder(NiteCompilation niteCompilation, Binder parent, Scope scope) : base(niteCompilation, parent, scope)
	{
	}

	public override BoundNode Bind(SyntaxNode syntax)
	{
		if (syntax is StatementSyntax statement)
			return BindStatement(statement);

		throw new ArgumentException(null, nameof(syntax));
	}

	public BoundStatement BindStatement(StatementSyntax syntax)
	{
		return syntax switch
		{
			ReturnStatementSyntax @return => BindReturn(@return),
			ExpressionStatementSyntax expressionStatement => BindExpressionStatement(expressionStatement),
			_ => throw new ArgumentException(null, nameof(syntax))
		};
	}

	private BoundStatement BindExpressionStatement(ExpressionStatementSyntax statement)
	{
		ExpressionBinder binder = new(NiteCompilation, this);
		return new BoundExpressionStatement(statement, binder.BindExpression(statement.Expression));
	}

	private BoundStatement BindReturn(ReturnStatementSyntax statement)
	{
		if (statement.Expression != null)
		{
			ExpressionBinder binder = new(NiteCompilation, this);
			return new BoundReturnStatement(statement, binder.BindExpression(statement.Expression));
		}

		return new BoundReturnStatement(statement);
	}
}