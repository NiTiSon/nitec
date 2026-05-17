using System.IO;
using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.IntermediateRepresentation.Nir;

internal sealed class Temp(int id, TypeSymbol type) : Value
{
	public int Id { get; } = id;
	public override TypeSymbol Type { get; } = type;

	public override void Write(TextWriter writer)
	{
		writer.Write('%' + Id.ToString());
	}
}
