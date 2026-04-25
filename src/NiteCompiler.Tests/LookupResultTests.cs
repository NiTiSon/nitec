using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.Tests;

[TestFixture]
public class LookupResultTests
{
	[Test]
	public void Free_DoesNotReturnOwnedSymbolsBuilderToSharedPool()
	{
		LookupResult lookupResult = LookupResult.GetInstance();
		ArrayBuilder<Symbol> ownedSymbols = lookupResult.Symbols;
		lookupResult.Free();

		LookupResult reusedLookupResult = LookupResult.GetInstance();
		ArrayBuilder<Symbol> scratchBuilder = ArrayBuilder<Symbol>.GetInstance();
		try
		{
			Assert.That(reusedLookupResult.Symbols, Is.SameAs(ownedSymbols));
			Assert.That(scratchBuilder, Is.Not.SameAs(reusedLookupResult.Symbols));
		}
		finally
		{
			if (!ReferenceEquals(scratchBuilder, reusedLookupResult.Symbols))
			{
				scratchBuilder.Free();
			}

			reusedLookupResult.Free();
		}
	}
}
