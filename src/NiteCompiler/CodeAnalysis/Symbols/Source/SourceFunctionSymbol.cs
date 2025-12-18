using System.Collections.Immutable;
using NiteCompiler.CodeAnalysis.Declarations;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceFunctionSymbol : FunctionSymbol
{
	// private ImmutableArray<SourceParameterSymbol> _symbols;

	public override ImmutableArray<ParameterSymbol> Parameters { get; }
	public override string Name => Declaration.Name;
	public FunctionDeclaration Declaration { get; }
	public override ImmutableArray<Location> Locations => [Declaration.Syntax.Name.Location];

	public SourceFunctionSymbol(SourceModuleSymbol containingModule, FunctionDeclaration declaration) : base(containingModule)
	{
		Declaration = declaration;
	}

	// public SourceFunctionSymbol(SourceTypeSymbol containingModule) : base(containingModule)
	// {
	// }

	public override bool IsAbstract => Declaration.Modifiers.HasFlag(DeclarationModifiers.Abstract);
	public override bool IsOverride => Declaration.Modifiers.HasFlag(DeclarationModifiers.Override);
	public override bool IsSealed => Declaration.Modifiers.HasFlag(DeclarationModifiers.Sealed);
	public override bool IsVirtual => Declaration.Modifiers.HasFlag(DeclarationModifiers.Virtual);
}