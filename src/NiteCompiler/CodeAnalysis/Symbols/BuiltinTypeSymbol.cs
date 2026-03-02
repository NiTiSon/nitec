namespace NiteCompiler.CodeAnalysis.Symbols;

[ForRemoval("Temporal solution, in future will be replaced with standard type resolving phase.")]
public sealed class BuiltinTypeSymbol : TypeSymbol
{
	public override Symbol? ContainingSymbol => null;

	public override string Name { get; }

	public BuiltinTypeSymbol(string name) => Name = name;

	public static BuiltinTypeSymbol I32 = new BuiltinTypeSymbol("Int32");
}