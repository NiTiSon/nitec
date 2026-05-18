using System.IO;

namespace NiteCompiler.IntermediateRepresentation.Nir;

internal sealed class Copy(IValue value) : Operand
{
	public override IValue Value { get; } = value;
	public override bool IsMove => false;

	public override void Write(TextWriter writer)
	{
		Value.Write(writer);
	}
}
