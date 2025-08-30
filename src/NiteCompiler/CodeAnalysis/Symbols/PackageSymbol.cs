namespace NiteCompiler.CodeAnalysis.Symbols;

public sealed class PackageSymbol : Symbol, INamedSymbol
{
    public string Name { get; }

    public PackageSymbol(string name)
    {
        Name = name;
    }

    public override Symbol? ContainingSymbol => null;
    public override SymbolKind Kind => SymbolKind.Package;
}