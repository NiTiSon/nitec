using System.Collections.Generic;
using System.Linq;

namespace NiteCompiler.CodeAnalysis.Symbols;

public abstract class LibrarySymbol : Symbol, INamedSymbol, IContainerSymbol
{
	public abstract override string Name { get; }
	public sealed override SymbolKind Kind => SymbolKind.Library;
	public abstract IEnumerable<ModuleSymbol> Modules { get; }
	public virtual IEnumerable<TypeSymbol> AllTypes
	{
		get
		{
			return Modules.SelectMany(t => t.GetMembersRecursively(false)).OfType<TypeSymbol>();
		}
	}
	IEnumerable<IMemberSymbol> IContainerSymbol.Members => Modules;

	private protected LibrarySymbol() {}
}