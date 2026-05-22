using System.Collections.Immutable;

namespace NiteCompiler.CodeAnalysis.Symbols;

public class AttributeSymbol : Symbol
{
	public override string Name { get; }
	public override Symbol? ContainingSymbol { get; }
	public override SymbolKind Kind => SymbolKind.Attribute;
	public ImmutableArray<ParameterSymbol> Parameters { get; }
	public AttributeTargets Targets { get; }

	internal AttributeSymbol(string name, Symbol containingSymbol,
		ImmutableArray<ParameterSymbol> parameters, AttributeTargets targets)
	{
		Name = name;
		ContainingSymbol = containingSymbol;
		Parameters = parameters;
		Targets = targets;
	}

	public override string ToDisplayString(SymbolFormat format = SymbolFormat.Default)
	{
		return Name;
	}

	public override void Accept(SymbolVisitor visitor)
	{
		visitor.VisitAttribute(this);
	}

	public override TResult? Accept<TResult>(SymbolVisitor<TResult> visitor)
		where TResult : default
	{
		return visitor.VisitAttribute(this);
	}

	public override TResult? Accept<TResult, TArgument>(SymbolVisitor<TResult, TArgument> visitor, TArgument arg)
		where TResult : default
	{
		return visitor.VisitAttribute(this, arg);
	}
}
