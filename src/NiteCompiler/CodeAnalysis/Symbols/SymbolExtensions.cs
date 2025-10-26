using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NiteCompiler.CodeAnalysis.Symbols;

public static class SymbolExtensions
{
	extension<T>(T container)
		where T : Symbol, IContainerSymbol
	{
		public IEnumerable<IMemberSymbol> GetMembersRecursively()
		{
			return GetMembersRecursivelyImpl(container);
		}
	}

	extension<T>(T container)
		where T : Symbol, IContainerSymbol, IMemberSymbol
	{
		public IEnumerable<IMemberSymbol> GetMembersRecursively(bool includeThis = false)
		{
			if (includeThis) yield return container;

			foreach (var member in GetMembersRecursively(container))
			{
				yield return member;
			}
		}
	}

	private static IEnumerable<IMemberSymbol> GetMembersRecursivelyImpl(IContainerSymbol container)
	{
		foreach (var member in container.Members)
		{
			yield return member;

			if (member is IContainerSymbol memberContainer)
			{
				foreach (var childContainer in GetMembersRecursivelyImpl(memberContainer))
				{
					yield return childContainer;
				}
			}
		}
	}
}