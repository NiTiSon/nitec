namespace NiteCompiler.CodeAnalysis.Symbols;

public sealed class PredefinedTypeSymbol : TypeSymbol
{
	internal PredefinedTypeSymbol(ModuleSymbol module, string name, DefaultType associatedDefaultType)
	{
		ContainingSymbol = module;
		Name = name;
		AssociatedDefaultType = associatedDefaultType;
	}

	public override ModuleSymbol ContainingSymbol { get; }
	public override string Name { get; }
	public DefaultType AssociatedDefaultType { get; }
}