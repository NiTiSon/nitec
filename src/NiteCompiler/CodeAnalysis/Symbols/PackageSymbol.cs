namespace NiteCompiler.CodeAnalysis.Symbols;

public sealed class PackageSymbol : Symbol, INamedSymbol
{
    public string Name { get; }

    public PackageSymbol(string name)
    {
        Name = name;
    }
}