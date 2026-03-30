using System.Collections.Immutable;
using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Symbols;
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
		// TODO: Build locals
		return [];
	}
}