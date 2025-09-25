namespace NiteCompiler.CodeAnalysis.Symbols;

public sealed class BuiltinTypeSymbol : TypeSymbol
{
	private BuiltinTypeSymbol()
	{
	}

	public static readonly BuiltinTypeSymbol Void = new();
	public static readonly BuiltinTypeSymbol SInt8 = new();
	public static readonly BuiltinTypeSymbol SInt16 = new();
	public static readonly BuiltinTypeSymbol SInt32 = new();
	public static readonly BuiltinTypeSymbol SInt64 = new();
	public static readonly BuiltinTypeSymbol UInt8 = new();
	public static readonly BuiltinTypeSymbol UInt16 = new();
	public static readonly BuiltinTypeSymbol UInt32 = new();
	public static readonly BuiltinTypeSymbol UInt64 = new();
	public static readonly BuiltinTypeSymbol Float16 = new();
	public static readonly BuiltinTypeSymbol Float32 = new();
	public static readonly BuiltinTypeSymbol Float64 = new();

	public override Symbol? ContainingSymbol => null;
	public override SymbolKind Kind => SymbolKind.Type;
}