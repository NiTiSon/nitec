namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class AddressTypeSymbol : TypeSymbol
{
    public TypeSymbol BaseType { get; }
    public bool IsMutable { get; }
    public bool IsNullable { get; }

    protected AddressTypeSymbol(TypeSymbol baseType, bool mutable, bool nullable)
    {
        BaseType = baseType;
        IsMutable = mutable;
        IsNullable = nullable;
    }
}