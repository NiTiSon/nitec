using System;
using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class FunctionBinder : ScopedBinder
{
	public FunctionSymbol Function { get; }

	public FunctionBinder(Compilation compilation, Binder parent, FunctionSymbol function) : base(compilation, parent, new())
	{
		Function = function;
	}

	public override BoundNode Bind(SyntaxNode syntax)
	{
		return syntax switch
		{
			FunctionDeclarationSyntax function => BindFunction(function),
			BlockStatementSyntax block => BindBlock(block),
			_ => throw new ArgumentException(null, nameof(syntax))
		};
	}

	private BoundNode BindBlock(BlockStatementSyntax syntax)
	{
		var statements = ImmutableArray.CreateBuilder<BoundStatement>();
		StatementBinder binder = new(Compilation, this, new Scope(this.Scope));
		foreach (var s in syntax.Statements)
			statements.Add(binder.BindStatement(s));

		return new BoundBlockStatement(syntax, statements.ToImmutable());
	}

	private BoundNode BindFunction(FunctionDeclarationSyntax syntax)
	{
		return Bind(syntax.Block);
	}

	private class StatementBinder : ScopedBinder
	{
		public StatementBinder(Compilation compilation, Binder parent, Scope scope) : base(compilation, parent, scope)
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
			ExpressionBinder binder = new(Compilation, this);
			return new BoundExpressionStatement(statement, binder.BindExpression(statement.Expression));
		}

		private BoundStatement BindReturn(ReturnStatementSyntax statement)
		{
			if (statement.Expression != null)
			{
				ExpressionBinder binder = new(Compilation, this);
				return new BoundReturnStatement(statement, binder.BindExpression(statement.Expression));
			}

			return new BoundReturnStatement(statement);
		}
	}

	private class ExpressionBinder : Binder
	{
		public ExpressionBinder(Compilation compilation, Binder parent) : base(compilation, parent)
		{
		}

		protected override Symbol? LookupSymbol(string name, LookupOptions options = LookupOptions.Default)
		{
			return LookupSymbolInParent(name, options);
		}

		public override BoundNode Bind(SyntaxNode syntax)
		{
			if (syntax is ExpressionSyntax expression)
				return BindExpression(expression);

			throw new ArgumentException(null, nameof(syntax));
		}

		public BoundExpression BindExpression(ExpressionSyntax syntax)
		{
			return syntax switch
			{
				//IdentifierExpressionSyntax id => BindIdentifier(id),
				LiteralExpressionSyntax literal => BindLiteral(literal),
				UnaryExpressionSyntax unary => BindUnary(unary),
				BinaryExpressionSyntax binary => BindBinary(binary),
				//CallExpressionSyntax call => BindCall(call),
				_ => throw new InvalidOperationException("Unknown expression syntax")
			};
		}

		private BoundExpression BindLiteral(LiteralExpressionSyntax syntax)
		{
			if (syntax.Token is not ITokenWithValue value) throw new ArgumentException(null, nameof(syntax));

			TypeSymbol type = Compilation.GlobalScope.GetPredefinedType(value.Type);

			return new BoundLiteralExpression(syntax, type, value);
		}

		// private BoundExpression BindIdentifier(IdentifierExpressionSyntax id)
		// {
		// 	var symbol = LookupSymbol(id.Name)
		// 	             ?? throw new Exception($"Unknown identifier {id.Name}");
		//
		//
		// 	return symbol switch
		// 	{
		// 		LocalVariableSymbol local => new BoundLocalVariable(local),
		// 		FieldSymbol field => new BoundFieldAccess(field),
		// 		ParameterSymbol param => new BoundParameter(param),
		// 		TypeSymbol t => throw new Exception("Type used as value"),
		// 		_ => throw new Exception("Invalid identifier symbol type.")
		// 	};
		// }

		private BoundExpression BindUnary(UnaryExpressionSyntax syntax)
		{
			BoundExpression value = BindExpression(syntax.Expression);

			return new BoundUnaryExpression(syntax, null, value);
		}

		private BoundExpression BindBinary(BinaryExpressionSyntax syntax)
		{
			BoundExpression left = BindExpression(syntax.Left);
			BoundExpression right = BindExpression(syntax.Right);

			//var op = BoundBinaryOperator.Bind(syntax.OperatorToken, left.Type, right.Type);
			//if (op == null)
			//	throw new Exception($"Operator {syntax.OperatorToken} not defined for '{left.Type.Name}' and '{right.Type.Name}'");

			return new BoundBinaryExpression(syntax, left, null, right);
		}
	}
}