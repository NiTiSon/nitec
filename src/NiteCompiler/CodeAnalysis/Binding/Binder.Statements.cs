using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Binding;

internal partial class Binder
{
	public virtual BoundNode BindFunctionBody(SyntaxNode syntax, BindingDiagnosticBag diagnostics)
	{
		switch (syntax)
		{
			case FunctionDeclarationSyntax function:
				// if (function.Kind == NodeKind.ConstructorDeclaration)
				// {
				// 	return BindConstructorBody((ConstructorDeclarationSyntax)method, diagnostics);
				// }
				// if (function.Kind == NodeKind.NamedConstructorDeclaration)
				// {
				// 	return BindConstructorBody((NamedConstructorDeclarationSyntax)method, diagnostics);
				// }

				return BindFunctionBody(function, function.Body, diagnostics);
			default:
				throw new ArgumentException($"Unexpected syntax kind: {syntax.Kind}");
		}
	}

	private BoundNode BindFunctionBody(FunctionDeclarationSyntax function, FunctionBodySyntax body,
		BindingDiagnosticBag diagnostics)
	{
		return new BoundFunctionBody(function, (BoundBlock)BindMethodBodyStatement(body, diagnostics));
	}

	private BoundNode BindMethodBodyStatement(FunctionBodySyntax syntax, BindingDiagnosticBag diagnostics)
	{
		switch (syntax)
		{
			case EmptyFunctionBodySyntax empty:
				return BindSemicolonAsEmptyBlock(empty.SemicolonToken);
			case BlockFunctionBodySyntax block:
				return BindStatement(block.Block, diagnostics);
			default:
				throw new ArgumentException($"Unexpected syntax kind: {syntax.Kind}");
		}
	}

	private BoundNode BindSemicolonAsEmptyBlock(Token semicolon)
	{
		return new BoundBlock(semicolon, []);
	}

	private BoundStatement BindStatement(StatementSyntax syntax, BindingDiagnosticBag diagnostics, bool embedded = false)
	{
		NodeKind kind = syntax.Kind;

		if (kind == NodeKind.BlockStatement)
		{
			return BindBlock((BlockStatementSyntax)syntax, diagnostics);
		}

		if (kind == NodeKind.ExpressionStatement)
		{
			return BindExpressionStatement((ExpressionStatementSyntax)syntax, diagnostics);
		}

		if (kind == NodeKind.EmptyStatement)
		{
			return BindEmptyStatement((EmptyStatementSyntax)syntax, diagnostics);
		}

		if (kind == NodeKind.ReturnStatement)
		{
			return BindReturn((ReturnStatementSyntax)syntax, diagnostics);
		}

		if (kind == NodeKind.IfStatement)
		{
			return BindIf((IfStatementSyntax)syntax, diagnostics);
		}

		if (kind == NodeKind.LoopStatement)
		{
			return BindLoop((LoopStatementSyntax)syntax, diagnostics);
		}

		if (kind == NodeKind.WhileStatement)
		{
			return BindWhile((WhileStatementSyntax)syntax, diagnostics);
		}

		throw new ArgumentException($"Unexpected syntax kind: {kind}");
	}

	private BoundIfStatement BindIf(IfStatementSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		// TODO: BindBooleanExpression
		BoundExpression condition = BindValue(syntax.Condition, diagnostics, BindValueKind.RValue);
		BoundStatement then = BindStatement(syntax.ThenStatement, diagnostics, embedded: true);

		BoundStatement? @else = null;
		if (syntax.ElseClause is not null)
		{
			@else = BindStatement(syntax.ElseClause.ElseStatement, diagnostics, embedded: true);
		}

		return new BoundIfStatement(syntax, condition, then, @else);
	}

	private BoundReturn BindReturn(ReturnStatementSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		BoundExpression? arg = null;

		if (syntax.Expression != null)
		{
			arg = BindValue(syntax.Expression, diagnostics, BindValueKind.RValue);
		}

		return new BoundReturn(syntax, arg);
	}

	private BoundBlock BindBlock(BlockStatementSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		var binder = GetBinder(syntax);
		Debug.Assert(binder != null);

		return binder.BindBlockParts(syntax, diagnostics);
	}

	private BoundBlock BindBlockParts(BlockStatementSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		var syntaxStatements = syntax.Statements;
		int statementCount = syntaxStatements.Count;

		var boundStatements = ArrayBuilder<BoundStatement>.GetInstance(statementCount);

		for (int i = 0; i < statementCount; i++)
		{
			var boundStatement = BindStatement(syntaxStatements[i], diagnostics);
			boundStatements.Add(boundStatement);
		}

		return new BoundBlock(syntax, boundStatements.ToImmutableAndFree());
	}

	private BoundExpressionStatement BindExpressionStatement(ExpressionStatementSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		BoundExpression expression = BindExpression(syntax.Expression, diagnostics, false, false);

		return new BoundExpressionStatement(syntax, expression);
	}

	private BoundStatement BindEmptyStatement(EmptyStatementSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		throw new NotImplementedException();
	}

	private BoundLoopStatement BindLoop(LoopStatementSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		BoundStatement body = BindStatement(syntax.Body, diagnostics, embedded: true);

		return new BoundLoopStatement(syntax, body);
	}

	private BoundWhileStatement BindWhile(WhileStatementSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		BoundExpression condition = BindRValueWithoutTargetType(syntax.Condition, diagnostics);
		BoundStatement body = BindStatement(syntax.Body, diagnostics, embedded: true);

		return new BoundWhileStatement(syntax, condition, body);
	}
}