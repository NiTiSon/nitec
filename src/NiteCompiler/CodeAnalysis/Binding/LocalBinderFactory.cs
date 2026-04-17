using System.Collections.Generic;
using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class LocalBinderFactory : SyntaxVisitor
{
	private readonly Dictionary<SyntaxNode, Binder> _map = new();
	private Binder _enclosing;

	public static Dictionary<SyntaxNode, Binder> Build(
		Symbol containingSymbol,
		SyntaxNode syntax,
		Binder root)
	{
		var factory = new LocalBinderFactory(root);

		if (syntax is StatementSyntax && syntax.Kind != NodeKind.BlockStatement)
		{
			throw new UnreachableException();
		}
		else if (syntax is BlockStatementSyntax block)
		{
			factory.Visit(block);
		}

		return factory._map;
	}

	private LocalBinderFactory(Binder root)
	{
		_enclosing = root;
	}

	private void Visit(SyntaxNode syntax, Binder enclosing)
	{
		if (_enclosing == enclosing)
		{
			this.Visit(syntax);
		}
		else
		{
			Binder oldEnclosing = _enclosing;
			_enclosing = enclosing;
			this.Visit(syntax);
			_enclosing = oldEnclosing;
		}
	}

	public override void VisitBlockStatement(BlockStatementSyntax node)
	{
		var blockBinder = new BlockBinder(_enclosing, node);

		Add(node, blockBinder);

		Binder outer = _enclosing;
		_enclosing = blockBinder;

		foreach (var stmt in node.Statements)
		{
			Visit(stmt);
		}

		_enclosing = outer;
	}

	public override void VisitExpressionStatement(ExpressionStatementSyntax node)
	{
		Visit(node.Expression);
	}

	public override void VisitIfStatement(IfStatementSyntax statement)
	{
		Binder enclosing = _enclosing;
		while (true)
		{
			Visit(statement.Condition, enclosing);
			VisitPossibleEmbeddedStatement(statement.ThenStatement, enclosing);

			if (statement.ElseClause == null)
			{
				break;
			}

			var elseStatementSyntax = statement.ElseClause.ElseStatement;
			if (elseStatementSyntax is IfStatementSyntax ifStatementSyntax)
			{
				statement = ifStatementSyntax;
				enclosing = GetBinderForPossibleEmbeddedStatement(statement, enclosing);
			}
			else
			{
				VisitPossibleEmbeddedStatement(elseStatementSyntax, enclosing);
				break;
			}
		}
	}

	public override void VisitLoopStatement(LoopStatementSyntax statement)
	{
		Binder loopBinder = new LoopBinder(_enclosing, statement);
		Add(statement, loopBinder);

		Binder outer = _enclosing;
		_enclosing = loopBinder;

		Visit(statement.Body);

		_enclosing = outer;
	}

	public override void VisitWhileStatement(WhileStatementSyntax statement)
	{
		Binder enclosing = _enclosing;
		Visit(statement.Condition, enclosing);
		VisitPossibleEmbeddedStatement(statement.Body, enclosing);
	}

	public override void VisitReturnStatement(ReturnStatementSyntax node)
	{
		if (node.Expression != null)
			Visit(node.Expression);
	}

	public override void VisitUnaryExpression(UnaryExpressionSyntax node)
	{
		Visit(node.Expression);
	}

	public override void VisitBinaryExpression(BinaryExpressionSyntax node)
	{
		Visit(node.Left);
		Visit(node.Right);
	}

	private Binder GetBinderForPossibleEmbeddedStatement(StatementSyntax statement, Binder enclosing, out SyntaxNode? embeddedScopeDesignator)
	{
		if (statement.Kind == NodeKind.ExpressionStatement)
		{
			embeddedScopeDesignator = statement;
			return new EmbeddedStatementBinder(enclosing, statement);
		}

		if (statement.Kind == NodeKind.ReturnStatement)
		{
			embeddedScopeDesignator = statement;
			return new EmbeddedStatementBinder(enclosing, statement);
		}

		embeddedScopeDesignator = null;
		return enclosing;
	}

	private Binder GetBinderForPossibleEmbeddedStatement(StatementSyntax statement, Binder enclosing)
	{
		SyntaxNode? embeddedScopeDesignator;
		// Some statements by default do not introduce its own scope for locals.
		// For example: Expression Statement, Return Statement, etc. However,
		// when a statement like that is an embedded statement (like IfStatementSyntax.Statement),
		// then it should introduce a scope for locals declared within it. Here we are detecting
		// such statements and creating a binder that should own the scope.
		enclosing = GetBinderForPossibleEmbeddedStatement(statement, enclosing, out embeddedScopeDesignator);

		if (embeddedScopeDesignator is not null)
		{
			Add(embeddedScopeDesignator, enclosing);
		}

		return enclosing;
	}

	private void VisitPossibleEmbeddedStatement(StatementSyntax? statement, Binder enclosing)
	{
		if (statement is not null)
		{
			enclosing = GetBinderForPossibleEmbeddedStatement(statement, enclosing);
			Visit(statement, enclosing);
		}
	}

	private void Add(SyntaxNode node, Binder binder)
	{
		_map[node] = binder;
	}
}