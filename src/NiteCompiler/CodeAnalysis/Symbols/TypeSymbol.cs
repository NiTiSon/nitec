using System.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class TypeSymbol : ContainerSymbol
{
	public sealed override SymbolKind Kind => SymbolKind.Type;

	public virtual SpecialType SpecialType => SpecialType.None;

	public bool IsVoidType => SpecialType == SpecialType.StdVoid;
	public bool IsErrorType => this is IErrorType;

	public override string ToDisplayString(SymbolFormat format = SymbolFormat.Default)
	{
		Debug.Assert(format.IsValid);
		if (ContainingSymbol is LibrarySymbol)
		{
			return Name;
		}

		return ContainingSymbol!.ToDisplayString() + "::" + Name;
	}

	public override void Accept(SymbolVisitor visitor)
	{
		visitor.VisitType(this);
	}

	public override TResult? Accept<TResult>(SymbolVisitor<TResult> visitor) where TResult : default
	{
		return visitor.VisitType(this);
	}

	public override TResult? Accept<TResult, TArgument>(SymbolVisitor<TResult, TArgument> visitor, TArgument arg) where TResult : default
	{
		return visitor.VisitType(this, arg);
	}
}