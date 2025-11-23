using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace NiteCompiler.CodeAnalysis.Symbols;

public sealed class GenericParameterSymbol : Symbol
{
	public override string Name { get; }
	public override SymbolKind Kind =>  SymbolKind.GenericParameter;
	public override Symbol ContainingSymbol { get; }
	[MemberNotNullWhen(false, nameof(IsConstantParameter))]
	public TypeSymbol? ValueType { get; }
	public bool IsConstantParameter => ValueType == null;

	public GenericParameterSymbol(string name, Symbol containingSymbol, [Optional] TypeSymbol? valueType)
	{
		Name = name;
		ContainingSymbol = containingSymbol;
		ValueType = valueType;
	}
}