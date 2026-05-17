using System.IO;
using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.IntermediateRepresentation.Nir;

internal abstract class Value : IValue
{
	public abstract TypeSymbol Type { get; }
	public abstract void Write(TextWriter writer);
}
