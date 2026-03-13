using System.Collections.Immutable;

namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class ModuleSymbol : ContainerSymbol
{
	public sealed override SymbolKind Kind => SymbolKind.Module;

	public override void Accept(SymbolVisitor visitor)
	{
		visitor.VisitModule(this);
	}

	public override TResult? Accept<TResult>(SymbolVisitor<TResult> visitor) where TResult : default
	{
		return visitor.VisitModule(this);
	}

	public override TResult? Accept<TResult, TArgument>(SymbolVisitor<TResult, TArgument> visitor, TArgument arg) where TResult : default
	{
		return visitor.VisitModule(this, arg);
	}

	public abstract ImmutableArray<Symbol> GetMembersUnordered();
}