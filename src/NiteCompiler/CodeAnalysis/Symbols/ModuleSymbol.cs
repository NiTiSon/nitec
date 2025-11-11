using System.Collections.Generic;
using System.Linq;
using LLVMSharp;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class ModuleSymbol : Symbol, IMemberSymbol, IContainerSymbol, INamedSymbol
{
	public abstract string Name { get; }
	public sealed override SymbolKind Kind => SymbolKind.Module;
	public abstract IEnumerable<IMemberSymbol> Members { get; }
	public virtual IEnumerable<TypeSymbol> Types => Members.OfType<TypeSymbol>();
	public abstract LibrarySymbol? Library { get; }
	IContainerSymbol? IMemberSymbol.ContainingSymbol => Library;

	private protected ModuleSymbol()
	{
	}
}