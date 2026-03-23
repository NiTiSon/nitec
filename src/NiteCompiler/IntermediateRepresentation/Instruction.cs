using System.IO;

namespace NiteCompiler.IntermediateRepresentation;

internal abstract class Instruction
{
	public abstract void Emit(BinaryWriter writer);
}