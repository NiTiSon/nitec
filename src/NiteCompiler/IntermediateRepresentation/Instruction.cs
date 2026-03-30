using System.IO;

namespace NiteCompiler.IntermediateRepresentation;

internal abstract class Instruction
{
	public abstract bool IsBranch { get; }
	public abstract void Emit(BinaryWriter writer);
}