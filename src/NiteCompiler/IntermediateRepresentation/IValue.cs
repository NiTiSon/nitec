using System.IO;
using NiteCompiler.CodeAnalysis.Symbols;

namespace NiteCompiler.IntermediateRepresentation;

internal interface IValue
{
	TypeSymbol Type { get; }
	void Write(TextWriter writer);
}
