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

	internal override LifetimeSymbol ScopeLifetime
	{
		get
		{
			if (field == null)
			{
				ScopeLifetimeSymbol lifetime = new(this);
				Interlocked.CompareExchange(ref field, lifetime, null);
			}

			return field;
		}
	}

	private sealed class ScopeLifetimeSymbol(Binder binder) : LifetimeSymbol
	{
		public override Symbol? ContainingSymbol { get; } = binder.ContainingMember;
		public LifetimeSymbol? ContainingLifetime { get; } = binder.Parent?.ScopeLifetime;

		public override bool Outlives(LifetimeSymbol other)
		{
			throw new NotImplementedException();
		}
	}
}