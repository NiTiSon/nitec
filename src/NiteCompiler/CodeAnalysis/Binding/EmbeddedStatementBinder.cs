using System;
using System.Collections.Immutable;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Symbols.Source;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class EmbeddedStatementBinder : LocalScopeBinder
{
	private readonly StatementSyntax _statement;

	public EmbeddedStatementBinder(Binder enclosing, StatementSyntax statement)
		: base(enclosing, enclosing.Flags)
	{
		Debug.Assert(statement != null);
		_statement = statement;
	}

	protected override ImmutableArray<LocalVariableSymbol> BuildLocals()
	{
		return [];
	}

	private ImmutableArray<LocalVariableSymbol> BuildLocals(SyntaxList<StatementSyntax> statements, Binder enclosingBinder)
	{
#if DEBUG
		Binder currentBinder = enclosingBinder;

		while (true)
		{
			if (this == currentBinder)
			{
				break;
			}

			currentBinder = currentBinder.Parent!;
		}
#endif

		var locals = ArrayBuilder<LocalVariableSymbol>.GetInstance(8);
		foreach (var statement in statements)
		{
			BuildLocals(enclosingBinder, statement, locals);
		}

		return locals.ToImmutableAndFree();
	}

	private void BuildLocals(Binder enclosingBinder, StatementSyntax statement, ArrayBuilder<LocalVariableSymbol> locals)
	{
		if (statement.Kind == NodeKind.LocalVariableDeclarationStatement)
		{
			Binder localDeclarationBinder = enclosingBinder.GetBinder(statement) ?? enclosingBinder;
			var declaration = (LocalVariableDeclarationStatement)statement;

			locals.Add(CreateLocal(declaration, declaration.Declarator, localDeclarationBinder));
		}
	}

	private SourceLocalVariableSymbol CreateLocal(LocalVariableDeclarationStatement syntax,
		LocalVariableDeclarator declarator,
		Binder? binder)
	{
		throw new NotImplementedException();
	}
}