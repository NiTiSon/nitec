using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Numerics;
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
}