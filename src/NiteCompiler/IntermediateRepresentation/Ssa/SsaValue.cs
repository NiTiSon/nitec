using System.IO;
using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.IntermediateRepresentation.Ssa;

internal abstract class SsaValue
{
	public TypeSymbol Type { get; }

	protected SsaValue(TypeSymbol type)
	{
		Type = type;
	}

	public abstract void Write(TextWriter writer);
}