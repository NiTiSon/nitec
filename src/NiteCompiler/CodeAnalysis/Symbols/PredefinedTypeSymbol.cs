namespace NiteCompiler.CodeAnalysis.Symbols;

public sealed class PredefinedTypeSymbol : TypeSymbol
{
    public static readonly PredefinedTypeSymbol U8, U16, U32, U64;
    public static readonly PredefinedTypeSymbol I8, I16, I32, I64;

    private PredefinedTypeSymbol()
    {

    }

    static PredefinedTypeSymbol()
    {
        U8 = U16 = U32 = U64 = new PredefinedTypeSymbol();
        I8 = I16 = I32 = I64 = new PredefinedTypeSymbol();
    }
}