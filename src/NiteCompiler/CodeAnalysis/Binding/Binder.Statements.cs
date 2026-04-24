using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Symbols.Source;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Binding;

internal partial class Binder
{
	protected TypeSymbol GetCurrentReturnType()
	{
		if (ContainingMember is FunctionSymbol symbol)
		{
			TypeSymbol returnType = symbol.ReturnType;

			return returnType;
		}

		return null!;
	}


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
		return new BoundFunctionBody(function, (BoundBlock)BindFunctionBodyStatement(body, diagnostics));
	}

	private BoundNode BindFunctionBodyStatement(FunctionBodySyntax syntax, BindingDiagnosticBag diagnostics)
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
		return new BoundBlock(semicolon, [], []);
	}

	private BoundStatement BindStatement(StatementSyntax syntax, BindingDiagnosticBag diagnostics, bool embedded = false)
	{
		NodeKind kind = syntax.Kind;

		if (kind == NodeKind.ExpressionStatement)
		{
			return BindExpressionStatement((ExpressionStatementSyntax)syntax, diagnostics);
		}

		if (kind == NodeKind.ReturnStatement)
		{
			return BindReturn((ReturnStatementSyntax)syntax, diagnostics);
		}

		if (kind == NodeKind.LocalVariableDeclarationStatement)
		{
			return BindLocalVariableDeclaration((LocalVariableDeclarationStatement)syntax, diagnostics);
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

		if (kind == NodeKind.BlockStatement)
		{
			return BindBlock((BlockStatementSyntax)syntax, diagnostics);
		}

		if (kind == NodeKind.EmptyStatement)
		{
			return BindEmptyStatement((EmptyStatementSyntax)syntax, diagnostics);
		}

		throw new UnreachableException($"BindStatement({kind})");
	}

	private BoundStatement BindLocalVariableDeclaration(LocalVariableDeclarationStatement syntax,
		BindingDiagnosticBag diagnostics)
	{
		return BindLocalVariableDeclarator(syntax.Declarator, diagnostics);
	}

	private BoundStatement BindLocalVariableDeclarator(LocalVariableDeclarator declarator,
		BindingDiagnosticBag diagnostics)
	{
		SourceLocalVariableSymbol local = LocateDeclaredLocalVariableSymbol(declarator, diagnostics);

		BoundExpression? initializer = null;
		if (declarator.EqualsValueClause != null)
		{
			initializer = BindExpression(declarator.EqualsValueClause.Expression, diagnostics, false, false);
		}

		TypeSymbol? declaredType = null;
		if (declarator.TypeClause is not null)
		{
			declaredType = BindType(declarator.TypeClause.Type, diagnostics);
		}

		if (declaredType == null && initializer == null) // It's called: try to guess type or DIE 💀☠️🪦
		{
			diagnostics.Diagnostics.ReportImplicitlyTypedVariableMustBeInitialized(declarator.Name.Location);

				return new BoundLocalVariableDeclarationStatement(declarator, local, null, hasErrors: true);
		}

		if (declaredType != null && initializer != null)
		{
			if (initializer.Type != declaredType)
			{
				diagnostics.Diagnostics.ReportCannotImplicitlyConvert(initializer.Syntax!.Location, initializer.Type, declaredType);

				return new BoundLocalVariableDeclarationStatement(declarator, local, initializer, hasErrors: true);
			}
		}

		bool hasErrors = (initializer?.HasErrors ?? false) || local.Type.IsErrorType;

		return new BoundLocalVariableDeclarationStatement(declarator, local, initializer, hasErrors);
	}

	private SourceLocalVariableSymbol LocateDeclaredLocalVariableSymbol(LocalVariableDeclarator declarator,
		BindingDiagnosticBag diagnostics)
	{
		SimpleNameSyntax identifier = declarator.Name;
		SourceLocalVariableSymbol localSymbol = LookupLocalVariable(identifier);
		Debug.Assert(localSymbol != null);
		return localSymbol;
	}

	private BoundIfStatement BindIf(IfStatementSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		// TODO: BindBooleanExpression
		BoundExpression condition = BindBooleanExpression(syntax.Condition, diagnostics);
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
		TypeSymbol retType = GetCurrentReturnType();
		bool hasErrors = false;

		if (syntax.Expression != null) arg = BindExpression(syntax.Expression, diagnostics, false, false);

		// TODO[NOT-CRITICAL]: add NeverReturn case
		if (retType.IsVoidType) // func -> void
		{
			if (arg != null) // return EXPR;
			{
				diagnostics.Diagnostics.ReportCannotReportValue(arg.Syntax!.Location);
				hasErrors = true;
			}
		}
		else // func -> any_type_not_void
		{
			if (arg == null) // return void;
			{
				diagnostics.Diagnostics.ReportMustReturnValue(syntax.Location);
				hasErrors = true;
			}
			else // return EXPR;
			{
				// TODO: Conversion
				if (arg.Type != retType)
				{
					diagnostics.Diagnostics.ReportWrongReturnExpressionType(arg.Syntax!.Location, arg.Type, retType);
					hasErrors = true;
				}
			}
		}

		if (arg != null)
		{
			hasErrors |= arg.HasErrors || arg.Type.IsErrorType;
		}

		return new BoundReturn(syntax, arg, hasErrors);
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

		var locals = GetDeclaredLocalsForScope(syntax);

		return new BoundBlock(syntax, locals, boundStatements.ToImmutableAndFree());
	}

	private BoundExpressionStatement BindExpressionStatement(ExpressionStatementSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		BoundExpression expression = BindExpression(syntax.Expression, diagnostics, false, false);

		return new BoundExpressionStatement(syntax, expression);
	}

	private BoundStatement BindEmptyStatement(EmptyStatementSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		return new BoundEmptyStatement(syntax);
	}

	private BoundLoopStatement BindLoop(LoopStatementSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		BoundStatement body = BindStatement(syntax.Body, diagnostics, embedded: true);

		return new BoundLoopStatement(syntax, body);
	}

	private BoundWhileStatement BindWhile(WhileStatementSyntax syntax, BindingDiagnosticBag diagnostics)
	{
		BoundExpression condition = BindBooleanExpression(syntax.Condition, diagnostics);
		BoundStatement body = BindStatement(syntax.Body, diagnostics, embedded: true);

		return new BoundWhileStatement(syntax, condition, body);
	}
}