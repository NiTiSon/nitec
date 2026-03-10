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

		var prev = _enclosing;
		_enclosing = blockBinder;

		foreach (var stmt in node.Statements)
		{
			Visit(stmt);
		}

		_enclosing = prev;
	}

	public override void VisitExpressionStatement(ExpressionStatementSyntax node)
	{
		Visit(node.Expression);
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

	private void Add(SyntaxNode node, Binder binder)
	{
		_map[node] = binder;
	}
}