using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceLocalVariableSymbol : LocalVariableSymbol
{
	public bool IsAssignable { get; }
	public override string Name { get; }
	public override TypeSymbol Type { get; }
	public override SourceFunctionSymbol ContainingSymbol { get; }

	public SourceLocalVariableSymbol(SourceFunctionSymbol function, TypeSymbol type, bool isAssignable, string name,
		Location nameLocation, SyntaxReference syntaxReference)
	{
		ContainingSymbol = function;
		IsAssignable = isAssignable;
		Name = name;
		Type = type;
	}
}