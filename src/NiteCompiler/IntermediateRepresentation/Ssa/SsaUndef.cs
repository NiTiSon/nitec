using System.IO;
using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.IntermediateRepresentation.Ssa;

internal sealed class SsaUndef : SsaValue
{
	public SsaUndef(TypeSymbol type)
		: base(type)
	{
	}

	public override void Write(TextWriter writer)
	{
		writer.Write("undef");
	}
}
