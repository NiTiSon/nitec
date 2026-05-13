using System.IO;
using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.IntermediateRepresentation.Mir;

internal sealed class TempValue(int id, TypeSymbol type, string? name = null)
{
	public int Id { get; } = id;
	public string? Name { get; } = name;
	public TypeSymbol Type { get; } = type;

	public void Write(TextWriter writer)
	{
		writer.Write('%' + (Name ?? Id.ToString()));
	}
}