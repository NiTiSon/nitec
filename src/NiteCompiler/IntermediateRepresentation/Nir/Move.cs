using System.IO;

namespace NiteCompiler.IntermediateRepresentation.Nir;

internal sealed class Move(IValue value) : Operand
{
	public override IValue Value { get; } = value;
	public override bool IsMove => true;

	public override void Write(TextWriter writer)
	{
		writer.Write("move ");
		Value.Write(writer);
	}
}
