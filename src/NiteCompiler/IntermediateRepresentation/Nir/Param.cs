using System.IO;
using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.IntermediateRepresentation.Nir;

internal sealed class Param(ParameterSymbol parameter) : Value
{
	public ParameterSymbol Parameter { get; } = parameter;
	public override TypeSymbol Type => Parameter.Type;

	public override void Write(TextWriter writer)
	{
		writer.Write('%' + Parameter.Name);
	}
}
