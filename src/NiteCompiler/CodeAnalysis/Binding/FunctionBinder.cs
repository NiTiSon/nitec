using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Symbols.Source;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class FunctionBinder : Binder
{
	public SourceFunctionSymbol Function { get; }

	public FunctionBinder(Binder? parent, SourceFunctionSymbol function) : base(parent)
	{
		Function = function;
	}

	protected override Symbol? Lookup(string name, LookupOptions options = LookupOptions.Default)
	{
		// TODO: Locals and parameters include

		return LookupInContaining(name, options);
	}

	public void Bind()
	{
		if (Function.Syntax is null) return;

		BoundBlockStatement block = BindBlockStatement(Function.Syntax.Block);
	}

	private BoundBlockStatement BindBlockStatement(BlockStatementSyntax syntax)
	{
		var builder = ImmutableArray.CreateBuilder<BoundStatement>(syntax.Statements.Length);
		foreach (StatementSyntax node in syntax.Statements)
		{
			switch (node)
			{
				case ExpressionStatementSyntax expressionStatement:
					builder.Add(BindExpressionStatement(expressionStatement));
					break;
				case BlockStatementSyntax blockStatement:
					builder.Add(BindBlockStatement(blockStatement));
					break;
				case ReturnStatementSyntax returnStatement:
					builder.Add(BindReturnStatement(returnStatement));
					break;
			}
		}

		ImmutableArray<BoundStatement> statements = builder.ToImmutable();
		return new BoundBlockStatement(syntax, statements);
	}

	private BoundReturnStatement BindReturnStatement(ReturnStatementSyntax syntax)
	{
		BoundExpression? expression = syntax.Expression is null ? null : BindExpression(syntax.Expression);

		return new BoundReturnStatement(syntax, expression);
	}

	private BoundExpressionStatement BindExpressionStatement(ExpressionStatementSyntax syntax)
	{
		BoundExpression expression = BindExpression(syntax.Expression);
		return new BoundExpressionStatement(syntax, expression);
	}

	private BoundExpression BindExpression(ExpressionSyntax syntax)
	{
		switch (syntax)
		{
			case BinaryExpressionSyntax binaryExpression:
				return BindBinaryExpression(binaryExpression);
			case UnaryExpressionSyntax unaryExpression:
				return BindUnaryExpression(unaryExpression);
			case LiteralExpressionSyntax literalExpression:
				return BindLiteralExpression(literalExpression);
			default:
				throw new Exception("Unknown expression type.");
		}
	}

	private BoundBinaryExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
	{
		BoundExpression left = BindExpression(syntax.Left);
		// TODO: Bind operator
		BoundExpression right = BindExpression(syntax.Right);
		return new BoundBinaryExpression(syntax, left, null, right);
	}

	private BoundUnaryExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
	{
		BoundExpression value = BindExpression(syntax.Expression);
		// TODO: Bind operator
		return new BoundUnaryExpression(syntax, null, value);
	}

	private BoundLiteralExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
	{
		// TODO: Get value
		return new BoundLiteralExpression(syntax, 0);
	}
}