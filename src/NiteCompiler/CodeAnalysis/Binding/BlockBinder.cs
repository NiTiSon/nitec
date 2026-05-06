using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Compilation;

namespace NiteCompiler.CodeAnalysis.Binding;

internal sealed class BlockBinder : LocalScopeBinder
{
	private readonly BlockStatementSyntax _block;

	public BlockBinder(Binder parent, BlockStatementSyntax block) : base(parent)
	{
		_block = block;
	}

	protected override ImmutableArray<LocalVariableSymbol> BuildLocals()
	{
		return BuildLocals(this, _block.Statements);
	}


	internal override ImmutableArray<LocalVariableSymbol> GetDeclaredLocalsForScope(SyntaxNode scopeDesignator)
	{
		if (ScopeDesignator == scopeDesignator)
		{
			return Locals;
		}

		throw new UnreachableException();
	}

	internal override SyntaxNode ScopeDesignator => _block;
}